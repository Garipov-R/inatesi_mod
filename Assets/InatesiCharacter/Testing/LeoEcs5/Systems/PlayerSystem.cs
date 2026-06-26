using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject.SpaceFighter;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class PlayerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsFilter _PlayerCharacterFilter;
        private EcsFilter _PlayerFilter;
        private EcsFilter _DamageFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsPool<DamageComponent> _DamagePool; 
        private EcsFilter _PlayerInitFilter;
        private EcsFilter _CollisionEventFilter;
        private EcsPool<CollisionComponentEvent> _CollisionEventPool;

        private EcsWorld _ecsWorld;
        private SharedData _sharedData;


        public void Init(IEcsSystems systems)
        {
            _ecsWorld = systems.GetWorld(); 
            _sharedData = systems.GetShared<SharedData>();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _PlayerInitFilter = systems.GetWorld().Filter<PlayerInitEvent>().End();
            _CollisionEventFilter = systems.GetWorld().Filter<CollisionComponentEvent>().End();
            _CollisionEventPool = systems.GetWorld().GetPool<CollisionComponentEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (G.IsPause) return;


            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);



                if (playerComponent.inputEnabled == false)
                    continue;

                //characterComponent.characterMotion.UpdateCharacter();
                characterComponent.characterMotion.UpdateAnimator();
                characterComponent.characterMotion.UpdateFootstep();
                characterComponent.characterMotion.UpdateCharacterMethod();
                characterComponent.characterMotion.CheckWater();


                




                foreach (var entityDamage in _DamageFilter)
                {
                    ref var damageComponent = ref _DamagePool.Get(entityDamage);

                    if (characterComponent.health <= 0)
                        continue;

                    if (damageComponent.target == characterComponent.gameObject)
                    {
                        characterComponent.characterMotion.AddForce(damageComponent.velocity);
                        var clips = systems.GetShared<SharedData>().CharacterSO.AudioCharacter.HurtClips;
                        characterComponent.characterMotion.AudioSource.PlayOneShot(
                            clips[UnityEngine.Random.Range(0, clips.Length - 1)]
                        );

                        if (damageComponent.damage > 999)
                        {
                            characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnLandedClip );
                        }

                        characterComponent.health -= damageComponent.damage;

                        if (characterComponent.health <= 0)
                        {
                            //playerComponent.cameraMotion.InputEnabled = false;
                            //playerComponent.cameraMotion.Follow = null;
                            //playerComponent.cameraMotion.transform.position = characterComponent.transform.position + characterComponent.transform.up * .5f;
                            playerComponent.inputEnabled = false;
                            //characterComponent.characterMotion.Velocity = Vector3.zero;
                            characterComponent.characterMotion.InputVector = Vector3.zero;
                            characterComponent.characterMotion.InputDirection = Vector3.zero;
                            characterComponent.InventoryInteraction2.DisableCurrentWeapon();


                            ref var characterDeadEventComponent = ref ECSHelper.Create<CharacterDeadEvent>(systems.GetWorld());
                            characterDeadEventComponent.entity = characterEntity;
                            characterDeadEventComponent.gameObject = characterComponent.gameObject;
                        }

                        SendEventObjectPool.Send(
                            systems.GetWorld(),
                            characterComponent.CharacterSO.DamageVisualEffect.gameObject,
                            damageComponent.hit.point,
                            Quaternion.LookRotation(-damageComponent.ray.direction),
                            damageComponent.hit,
                        PoolType.Particle
                        );

                        if (characterComponent.CharacterSO.DamageMaterial != null)
                        {
                            ref var objectPoolSendEvent = ref ECSHelper.Create<ObjectPoolSendEvent>(systems.GetWorld());
                            objectPoolSendEvent.objectToSpawn = systems.GetShared<SharedData>().ParticleSettingsSO.MeshDecalPrefab.gameObject;
                            objectPoolSendEvent.poolType = PoolType.Particle;
                            objectPoolSendEvent.data = characterComponent.CharacterSO.DamageMaterial;

                            var isHit = Physics.Raycast(
                                damageComponent.hit.point,
                                damageComponent.ray.direction,
                                out RaycastHit hit,
                                5f,
                                Configs.Config.s_DefaultLayerMask,
                                QueryTriggerInteraction.Ignore
                            );

                            if (isHit)
                            {
                                objectPoolSendEvent.rotation = Quaternion.LookRotation(damageComponent.ray.direction);
                                objectPoolSendEvent.position = hit.point;
                                objectPoolSendEvent.parent = hit.transform;
                            }
                            else
                            {
                                var isHit2 = Physics.Raycast(
                                    damageComponent.hit.point,
                                    Vector3.down,
                                    out RaycastHit hit2,
                                    3f,
                                    Configs.Config.s_DefaultLayerMask,
                                    QueryTriggerInteraction.Ignore
                                );

                                if (isHit2)
                                {
                                    objectPoolSendEvent.rotation = Quaternion.LookRotation(-hit2.normal);
                                    objectPoolSendEvent.position = hit2.point;
                                    objectPoolSendEvent.parent = hit2.transform;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }

                foreach (var collicionEventEntity in _CollisionEventFilter)
                {
                    ref var collisionEvent = ref _CollisionEventPool.Get(collicionEventEntity);
                    
                    if (collisionEvent.collideGameObject == playerComponent.gameObject)
                    {
                        _sharedData.GameLogic.OnPlayerCollision(collisionEvent);

                        if (collisionEvent.ownerGameObject.TryGetComponent(out CarriableObject item))
                        {
                            var i= characterComponent.InventoryInteraction2.AddItem(item.ItemScriptableObject);
                            characterComponent.InventoryInteraction2.SetActiveInventoryItem(i.SlotIndex);
                            if (item.AudioClip) characterComponent.characterMotion.AudioSource.PlayOneShot(item.AudioClip);
                        }
                    }
                }
            }
        }
    }
}
