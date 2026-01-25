using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.Testing.Decals;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.LeoEcs4.PoolSystems;
using InatesiCharacter.Testing.Pooling;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
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
                    var obj = SpawnObject(objectPoolEventComponent.objectToSpawn, objectPoolEventComponent.position, objectPoolEventComponent.rotation, objectPoolEventComponent.poolType);

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
                            //decal.transform.position = particleEventComponent.hit.point - decal.transform.forward * (decal.transform.localScale.magnitude / 2);
                            var hits = Physics.RaycastAll(damageCom.hit.point, Vector3.down, 10f, Configs.Config.s_DefaultLayerMask, QueryTriggerInteraction.Ignore);
                            foreach (var hit in hits)
                            {
                                if (hit.transform != damageCom.target.transform && hit.transform != decal.transform)
                                {
                                    Debug.Log(hit.transform.name);
                                    decal.targetMesh = hit.transform;
                                    //decal.transform.rotation = Quaternion.LookRotation(-damageCom.hit.normal);
                                    decal.transform.eulerAngles = new Vector3(decal.transform.eulerAngles.x, Random.Range(0, 360f), decal.transform.eulerAngles.z);
                                }
                            }
                            //decal.material = material;
                            decal.Recalculate();
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

                ref var objectPoolManagerComponent = ref _ObjectPoolManagerPool.Get(1);

                if (objectPoolComponent.poolType == PoolType.Particle)
                {
                    foreach (var item in objectPoolManagerComponent.objectsPool.Values)
                    {
                        if (item.CountActive > 5)
                        {
                            //Debug.Log($"{item.CountAll} {item.CountActive} {objectPoolComponent.index} {entity} {_ObjectPoolComponentFilter.GetEntitiesCount()}");
                            if (objectPoolComponent.gameObject.activeSelf == true)
                            {
                                //Debug.Log($"rett {objectPoolComponent.lifeTime} {objectPoolComponent.gameObject.activeSelf}");
                                //ReturnObjectToPool(objectPoolComponent.gameObject, objectPoolComponent.poolType);
                            }
                            //ReturnObjectToPool(objectPoolComponent.gameObject, objectPoolComponent.poolType);
                        }
                    }
                }
                else if (objectPoolComponent.poolType == PoolType.Particle)
                {

                }
                

                if (objectPoolComponent.lifeTime >= 0)
                {
                    objectPoolComponent.lifeTime -= Time.deltaTime;

                    continue;
                }

                if (objectPoolComponent.lifeTime < 0 && objectPoolComponent.gameObject.activeSelf == true)
                {
                    //Debug.Log($"rett {objectPoolComponent.lifeTime} {objectPoolComponent.gameObject.activeSelf}");
                    ReturnObjectToPool(objectPoolComponent.gameObject, objectPoolComponent.poolType);
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

        private void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObject)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => CreateObject(prefab, pos, rot, poolType),
                OnGetObject,
                OnReleaseObject,
                OnDestroyObject
            );

            GetPoolManagerComponent().objectsPool.Add(prefab, pool);
        }

        private GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObject)
        {
            prefab.SetActive(false);

            GameObject obj = GameObject.Instantiate(prefab, pos, rot);

            prefab .SetActive(true);

            GameObject parentObject = SetParentObject(poolType);
            obj.transform.SetParent (parentObject.transform);


            int newEntity = _EcsWorld.NewEntity();
            ref var objectPoolComponent = ref _ObjectPoolComponentPool.Add(newEntity);
            objectPoolComponent.lifeTime = 1f;
            objectPoolComponent.gameObject = obj;
            objectPoolComponent.poolType = poolType;
            objectPoolComponent.index = newEntity;

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

        private T SpawnObject<T> (GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObject) where T : Object
        {
            if (objectToSpawn == null) return null;

            if (!GetPoolManagerComponent().objectsPool.ContainsKey(objectToSpawn))
            {
                CreatePool(objectToSpawn, spawnPos, spawnRot, poolType);
            }

            GameObject obj = GetPoolManagerComponent().objectsPool[objectToSpawn].Get();

            if (obj != null)
            {
                if (!GetPoolManagerComponent().clonePrefabMap.ContainsKey(obj))
                {
                    GetPoolManagerComponent().clonePrefabMap.Add(obj, objectToSpawn);
                }

                obj.transform.position = spawnPos;
                obj.transform.rotation = spawnRot;
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
            return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, poolType);
        }

        private GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.GameObject)
        {
            return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRot, poolType);
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
