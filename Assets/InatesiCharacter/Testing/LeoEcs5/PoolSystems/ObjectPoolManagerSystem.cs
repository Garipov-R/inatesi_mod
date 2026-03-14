using System;
using System.Collections.Generic;
using InatesiCharacter.Testing.Decals;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace InatesiCharacter.Testing.LeoEcs5.PoolSystems
{
    public class ObjectPoolManagerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _ObjectPoolManagerFilter;
        private EcsFilter _ObjectPoolEventFilter;
        private EcsPool<ObjectPoolManagerComponent> _ObjectPoolManagerPool;
        private EcsPool<ObjectPoolSendEvent> _ObjectPoolEventPool;
        private EcsFilter _ParticleEventFilter;
        private EcsPool<ParticleEvent> _ParticleEventPool;
        private EcsFilter _DamageFilter;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsFilter _ObjectPoolComponentFilter;
        private EcsPool<ObjectPoolComponent> _ObjectPoolComponentPool;

        private EcsWorld _EcsWorld;

        public void Init(IEcsSystems systems)
        {
            _ObjectPoolManagerPool = systems.GetWorld().GetPool<ObjectPoolManagerComponent>();
            _ObjectPoolEventPool = systems.GetWorld().GetPool<ObjectPoolSendEvent>();
            _ObjectPoolManagerFilter = systems.GetWorld().Filter<ObjectPoolManagerComponent>().End();
            _ObjectPoolEventFilter = systems.GetWorld().Filter<ObjectPoolSendEvent>().End();
            _ParticleEventFilter = systems.GetWorld().Filter<ParticleEvent>().End();
            _ParticleEventPool = systems.GetWorld().GetPool<ParticleEvent>();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _ObjectPoolComponentPool = systems.GetWorld().GetPool<ObjectPoolComponent>();
            _ObjectPoolComponentFilter = systems.GetWorld().Filter<ObjectPoolComponent>().End();

            var newEntity = systems.GetWorld().NewEntity();
            ref var objectPoolComponent = ref _ObjectPoolManagerPool.Add(newEntity);
            objectPoolComponent.objectsPool = new Dictionary<GameObject, ObjectPool<GameObject>>();
            objectPoolComponent.clonePrefabMap = new Dictionary<GameObject, GameObject>();

            _EcsWorld = systems.GetWorld();

            SetupEmpties();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _ObjectPoolEventFilter)
            {
                ref var objectPoolEventComponent = ref _ObjectPoolEventPool.Get(entity);

                try 
                {
                    GameObject obj = null;
                    if (objectPoolEventComponent.parent == null)
                    {
                        obj = SpawnObject(
                            objectPoolEventComponent.objectToSpawn, 
                            objectPoolEventComponent.position, 
                            objectPoolEventComponent.rotation, 
                            objectPoolEventComponent.poolType
                        );
                    }
                    else
                    {
                        obj = SpawnObject(
                            objectPoolEventComponent.objectToSpawn,
                            objectPoolEventComponent.position,
                            objectPoolEventComponent.rotation,
                            objectPoolEventComponent.parent,
                            objectPoolEventComponent.poolType
                        );

                    }

                    if (obj.GetComponent<VisualEffect>())
                    {
                        obj.GetComponent<VisualEffect>().Play();
                    }
                    else if (obj.GetComponent<MeshDecal>())
                    {
                        foreach (var damageEntity in _DamageFilter)
                        {
                            ref var damageCom = ref _DamagePool.Get(damageEntity);

                            if (damageCom.isHit == false) continue;

                            var decal = obj.GetComponent<MeshDecal>();

                            var direction = Vector3.zero;
                            var lookRotation = Vector3.zero;
                            var characterLayer = LayerMask.LayerToName(damageCom.target.layer);
                            //Debug.Log(characterLayer);
                            if ("Character" == characterLayer || "Player" == characterLayer || "CharacterHitCollider" == characterLayer)
                            {
                                direction = Vector3.down;
                                lookRotation = Vector3.down;
                            }
                            else
                            {
                                direction = damageCom.ray.direction;
                                lookRotation = -damageCom.hit.normal;
                            }

                            var sphereCast = Physics.SphereCast(
                                objectPoolEventComponent.position, 
                                .001f, 
                                direction, 
                                out RaycastHit hitInfo, 
                                1f, 
                                Configs.Config.s_DefaultLayerMask, 
                                QueryTriggerInteraction.Ignore
                            );

                            Debug.DrawRay(objectPoolEventComponent.position, direction, Color.red, 2, true);

                            if (sphereCast == true)
                            {
                                decal.material = objectPoolEventComponent.data != null ? objectPoolEventComponent.data as Material : decal.material;

                                decal.targetMesh = objectPoolEventComponent.parent;
                                decal.transform.rotation = Quaternion.LookRotation(lookRotation);
                                decal.transform.localEulerAngles = new Vector3(decal.transform.eulerAngles.x, decal.transform.eulerAngles.y, Random.Range(0, 360f));
                                decal.Recalculate();
                            }

                            var hits = Physics.RaycastAll(
                                objectPoolEventComponent.position, 
                                damageCom.ray.direction,  
                                10f, 
                                Configs.Config.s_DefaultLayerMask, 
                                QueryTriggerInteraction.Ignore
                            );

                            
                        }
                    }
                    else if (obj.TryGetComponent(out AudioSource audioSource))
                    {
                        if (objectPoolEventComponent.data != null)
                        {
                            audioSource.clip = (AudioClip)objectPoolEventComponent.data;
                            audioSource.Play(); 
                        }
                    }

                    foreach (var objectPoolEntity in _ObjectPoolComponentFilter)
                    {
                        ref var objectPoolComponent = ref _ObjectPoolComponentPool.Get(objectPoolEntity);

                        if (obj == objectPoolComponent.gameObject)
                        {
                            objectPoolComponent.lifeTime = 10;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }


            foreach (var entity in _ObjectPoolComponentFilter)
            {
                ref var objectPoolComponent = ref _ObjectPoolComponentPool.Get(entity);


                foreach (var poolManagerEntity in _ObjectPoolManagerFilter)
                {
                    ref var objectPoolManagerComponent = ref _ObjectPoolManagerPool.Get(poolManagerEntity);


                    if (objectPoolManagerComponent.objectsPool.TryGetValue(objectPoolComponent.prefab, out var objectPool))
                    {
                        if (objectPool.CountActive > 100)
                        {
                            if (objectPoolComponent.lifeTime > 0)
                            {
                                objectPoolComponent.lifeTime -= Time.deltaTime;
                            }

                            if (objectPoolComponent.gameObject != null)
                            {
                                if (objectPoolComponent.lifeTime <= 0 && objectPoolComponent.gameObject.activeSelf == true)
                                {
                                    ReturnObjectToPool(objectPoolComponent.gameObject, objectPoolComponent.poolType);
                                }
                            }
                        }
                    }
                }
                
            }
        }

        private void SetupEmpties()
        {
            foreach (var entity in _ObjectPoolManagerFilter)
            {
                ref var objectPoolComponent = ref _ObjectPoolManagerPool.Get(entity);

                objectPoolComponent.emptyHolder = new GameObject("Object Pools");

                objectPoolComponent.gameObjectEmpty = new GameObject("Game Objects");
                objectPoolComponent.gameObjectEmpty.transform.SetParent(objectPoolComponent.emptyHolder.transform);

                objectPoolComponent.particlesEmpty = new GameObject("Particle Effects");
                objectPoolComponent.particlesEmpty.transform.SetParent(objectPoolComponent.emptyHolder.transform);

                objectPoolComponent.soundFXEmpty = new GameObject("Sound FX");
                objectPoolComponent.soundFXEmpty.transform.SetParent(objectPoolComponent.emptyHolder.transform);
            }
        }

        private void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, PoolType poolType = PoolType.GameObject)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => CreateObject(prefab, pos, rot, parent, poolType),
                OnGetObject,
                OnReleaseObject,
                OnDestroyObject
            );

            GetPoolManagerComponent().objectsPool.Add(prefab, pool);
        }

        private GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, PoolType poolType = PoolType.GameObject)
        {
            prefab.SetActive(false);

            GameObject obj = null;

            prefab.SetActive(true);

            if (parent != null)
            {
                obj = GameObject.Instantiate(prefab, parent); 
                //obj.transform.localPosition = pos;
                //obj.transform.localRotation = rot;
                //obj.transform.localScale = Vector3.one;
            }
            else
            {
                obj = GameObject.Instantiate(prefab, pos, rot);
                GameObject parentObject = SetParentObject(poolType);
                obj.transform.SetParent(parentObject.transform);
            }

            obj.gameObject.name += $"_{Random.Range(0, 999)}";

            int newEntity = _EcsWorld.NewEntity();
            ref var objectPoolComponent = ref _ObjectPoolComponentPool.Add(newEntity);
            objectPoolComponent.lifeTime = 1f;
            objectPoolComponent.gameObject = obj;
            objectPoolComponent.poolType = poolType;
            objectPoolComponent.index = newEntity;
            objectPoolComponent.prefab = prefab;

            return obj;
        }

        private GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.GameObject:
                    return GetPoolManagerComponent().gameObjectEmpty;

                case PoolType.Particle: 
                    return GetPoolManagerComponent().particlesEmpty;

                case PoolType.SoundFx:
                    return GetPoolManagerComponent().soundFXEmpty;

                default:
                    return null;
            }
        }

        private T SpawnObject<T> (GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, Transform parent, PoolType poolType = PoolType.GameObject) where T : Object
        {
            if (objectToSpawn == null) return null;

            if (!GetPoolManagerComponent().objectsPool.ContainsKey(objectToSpawn))
            {
                CreatePool(objectToSpawn, spawnPos, spawnRot, parent, poolType);
            }

            GameObject obj = GetPoolManagerComponent().objectsPool[objectToSpawn].Get();

            if (obj != null)
            {
                if (!GetPoolManagerComponent().clonePrefabMap.ContainsKey(obj))
                {
                    GetPoolManagerComponent().clonePrefabMap.Add(obj, objectToSpawn);
                }

                if (parent != null)
                {
                    obj.transform.position = spawnPos;
                    obj.transform.rotation = spawnRot;
                    obj.transform.SetParent(parent, true);
                }
                else
                {
                    obj.transform.position = spawnPos;
                    obj.transform.rotation = spawnRot;
                }

                obj.SetActive(true);

                if (typeof(T) == typeof(GameObject))
                {
                    return obj as T;
                }

                T component = obj.GetComponent<T>();
                if (component == null)
                {
                    return null;
                }

                return component;
            }

            return null;
        }

        private T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObject) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, null, poolType);
        }

        private T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot, Transform parent, PoolType poolType = PoolType.GameObject) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, parent, poolType);
        }

        private GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObject)
        {
            return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRot, null, poolType);
        }
        

        private GameObject SpawnObject(GameObject objectToSpawn, Vector3 pos, Quaternion spawnRot, Transform parent, PoolType poolType = PoolType.GameObject)
        {
            return SpawnObject<GameObject>(objectToSpawn, pos, spawnRot, parent, poolType);
        }

        private void ReturnObjectToPool(GameObject gameObject, PoolType poolType = PoolType.GameObject)
        {
            if (GetPoolManagerComponent().clonePrefabMap.TryGetValue(gameObject, out GameObject prefab))
            {
                GameObject parentObject = SetParentObject(poolType);

                if (gameObject.transform.parent != parentObject.transform)
                {
                    gameObject.transform.SetParent(parentObject.transform);
                }

                if (GetPoolManagerComponent().objectsPool.TryGetValue(prefab, out ObjectPool<GameObject> pool))
                {
                    pool.Release(gameObject);
                }
            }
            else
            {
                Debug.LogError("null " + gameObject.name);
            }
        }

        private void OnGetObject(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        private void OnReleaseObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private void OnDestroyObject(GameObject gameObject) 
        {

        }

        private ref ObjectPoolManagerComponent GetPoolManagerComponent() 
        {
            int resultEntity = -1;

            foreach (var entity in _ObjectPoolManagerFilter)
            {
                resultEntity = entity;
            }

            return ref _ObjectPoolManagerPool.Get(resultEntity);
        }
    }
}
