using InatesiCharacter.SuperCharacter.MovementTypes;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs4.Events;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared.Components;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class SpawnCharacterSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsFilter _SpawnFilter;
        private EcsPool<SpawnComponentEvent> _SpawnComponentEventPool;
        private EcsFilter _CharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsWorld _World;

        public void Init(IEcsSystems systems)
        {
            _SpawnFilter = systems.GetWorld().Filter<SpawnComponentEvent>().End();
            _SpawnComponentEventPool = systems.GetWorld().GetPool<SpawnComponentEvent>();
            _World = systems.GetWorld(); 
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _SpawnFilter)
            {
                ref var spawnComponent = ref _SpawnComponentEventPool.Get(entity);

                if (spawnComponent.data is not CharacterSO)
                    continue;

                SpawnCharacter(ref spawnComponent);
            }
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var entity in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(entity);

                characterComponent.CharacterMotionBase.OnLanded.RemoveListener(CharacterOnLanded);
            }
        }

        public void SpawnCharacter(ref SpawnComponentEvent spawnComponentEvent)
        {
            GameObject prefab; Vector3 position; Quaternion rotation; object data;

            GameObject modelCharacter = null;

            prefab = spawnComponentEvent.gameObject;
            position = spawnComponentEvent.position;
            rotation = spawnComponentEvent.rotation;
            data = spawnComponentEvent.data;

            if (prefab == null) return;

            int entity = _World.NewEntity();
            EcsPool<CharacterComponent> pool = _World.GetPool<CharacterComponent>();
            ref CharacterComponent characterComponent = ref pool.Add(entity);

            if (spawnComponentEvent.isPlayer)
            {
                spawnComponentEvent.entity = entity;
            }


            var characterGameObject = GameObject.Instantiate(
                prefab,
                position,
                rotation
            );

            characterComponent.GameObject = characterGameObject;
            characterComponent.CharacterMotionBase = characterGameObject.GetComponent<InatesiCharacter.SuperCharacter.CharacterMotionBase>();
            characterComponent.ICharacter = characterGameObject.GetComponent<ICharacter>();
            characterComponent.CharacterMotionBase.SetPositionAndRotation(position, rotation);
            characterComponent.CharacterMotionBase.IsInputDisabled = true;
            characterComponent.InventoryInteraction = new Testing.Character.InteractionSystem.InventoryInteraction(new InatesiArch.InventorySystems.InventoryContainer(), characterComponent.CharacterMotionBase);
            characterComponent.Dead = false;


            characterComponent.CharacterMotionBase.SetMovementType(new Default());
            characterComponent.CharacterMotionBase.SpeedMove = characterComponent.CharacterMotionBase.MoveConfig.Speed;
            characterComponent.CharacterMotionBase.IsInputDisabled = false;
            characterComponent.CharacterMotionBase.OnLanded.AddListener(CharacterOnLanded);


            characterComponent.NavMeshPath = new NavMeshPath();

            if (characterComponent.GameObject.TryGetComponent(out NavMeshAgent component))
            {
                characterComponent.NavMeshAgent = component;
                characterComponent.NavMeshAgent.enabled = false;
            }
            if (characterComponent.GameObject.TryGetComponent(out NavMeshObstacle component2))
            {
                characterComponent.NavMeshObstacle = component2;
                characterComponent.NavMeshObstacle.enabled = false;
            }
            if (characterComponent.GameObject.TryGetComponent(out CharacterWorldInteractionSystem characterWorldInteractionSystem))
            {
                characterComponent.CharacterWorldInteractionSystem = characterWorldInteractionSystem;
            }






            if (data != null)
            {
                CharacterSO characterSO = ScriptableObject.Instantiate(data as CharacterSO);
                characterComponent.Health = characterSO.CharacterConfig.StartHealth;
                characterComponent.CharacterMotionBase.MoveConfig = characterSO.MoveConfig;

                characterComponent.CharacterSO = characterSO;


                Avatar avatarCharacter = null;

                if (characterComponent.CharacterSO.Model != null)
                {
                    modelCharacter = GameObject.Instantiate(characterComponent.CharacterSO.Model, characterComponent.GameObject.transform);
                    avatarCharacter = modelCharacter.TryGetComponent(out Animator animator) ? animator.avatar : null;
                }
                /*else if (characterComponent.CharacterSO.Prefab != null)
                {
                    gameObjectCharacter = GameObject.Instantiate(characterComponent.CharacterSO.Prefab, characterComponent.GameObject.transform);
                    avatarCharacter = gameObjectCharacter.TryGetComponent(out Animator animator) ? animator.avatar : null;
                }*/
                else
                {
                    var defaultModel = Resources.Load(Configs.Config.c_DefaultModel) as GameObject;
                    modelCharacter = GameObject.Instantiate(defaultModel, characterComponent.GameObject.transform);
                }

                modelCharacter.transform.localScale = modelCharacter.transform.localScale * characterSO.ScaleMagnitude;

                if (characterSO.Material != null)
                {
                    var renderer = modelCharacter.GetComponent<Renderer>();

                    if (renderer == null)
                    {
                        renderer = modelCharacter.GetComponentInChildren<Renderer>();
                    }

                    if (renderer != null)
                    {
                        renderer.material = new Material(characterSO.Material);
                    }
                }

                characterComponent.CharacterMotionBase.AnimatorMonitor.SetAvatar(avatarCharacter);
            }

            characterComponent.CharacterMotionBase.Awake();

            characterComponent.CharacterMotionBase.AnimatorMonitor.Animator.applyRootMotion = characterComponent.CharacterSO.RootMotion;

            if (characterComponent.CharacterSO.AnimatorController != null)
                characterComponent.CharacterMotionBase.AnimatorMonitor.Animator.runtimeAnimatorController = characterComponent.CharacterSO.AnimatorController;



            /*if (modelCharacter != null)
            {
                var ragdoller = new InatesiCharacter.Testing.Character.Ragdolls.Ragdoller(modelCharacter.transform, modelCharacter.transform.forward);
                var _ragdollProperties = new InatesiCharacter.Testing.Character.Ragdolls.RagdollProperties
                {
                    asTrigger = true,
                    isKinematic = false,
                    rigidAngularDrag = 0.3f,
                    rigidDrag = 0.3f,
                    cdMode = CollisionDetectionMode.Continuous,
                    onlyCollider = true,
                };
                int _ragdollTotalWeight = 60;
                ragdoller.ApplyRagdoll(_ragdollTotalWeight, _ragdollProperties);


                if (modelCharacter.TryGetComponent(out Animator animator))
                {
                    foreach (var child in animator.GetBoneTransform(HumanBodyBones.Hips).GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("CharacterHitCollider");
                    }
                }
            }*/
            
        }


        private void CharacterOnLanded()
        {
            foreach (var entity in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(entity);

                if (characterComponent.CharacterMotionBase.Velocity.y < Mathf.Abs(characterComponent.CharacterSO.MoveConfig.FallDamageVelocity) * -1)
                {
                    var entityDamage = _World.NewEntity();
                    ref var damageComponent = ref _World.GetPool<DamageComponent>().Add(entityDamage);
                    damageComponent.damage =
                        (Mathf.Abs(characterComponent.CharacterMotionBase.Velocity.y) *
                        characterComponent.CharacterSO.MoveConfig.FallDamageMultiply) *
                        characterComponent.CharacterSO.MoveConfig.FallDamage;
                    damageComponent.target = characterComponent.GameObject;
                }

                //characterComponent.ICharacter.UpdateCharacterMethod();
            }
        }
    }
}