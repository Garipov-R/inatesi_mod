using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.InatesiArch.WeaponsTest;
using InatesiCharacter.SuperCharacter;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class CharacterUpdateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsFilter _DamageFilter;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsWorld _world;
        private EcsFilter _DieFilter;
        private EcsPool<CharacterDieEvent> _DiePool;


        public void Init(IEcsSystems systems)
        {
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _world = systems.GetWorld();
            _DieFilter = systems.GetWorld().Filter<CharacterDieEvent>().End();
            _DiePool = systems.GetWorld().GetPool<CharacterDieEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;


            foreach (var entity in _DieFilter)
            {
                ref var dieComponent = ref _DiePool.Get(entity);

                if (_CharacterPool.Has(dieComponent.entityCharacter))
                {
                    ref var characterComponent = ref _CharacterPool.Get(dieComponent.entityCharacter);

                    //characterComponent.TimeOfDeath = Time.time;
                    characterComponent.CharacterMotionBase.IsInputDisabled = true;


                    var ragdoller = new InatesiCharacter.Testing.Character.Ragdolls.Ragdoller(characterComponent.GameObject.transform, characterComponent.GameObject.transform.forward);
                    var _ragdollProperties = new InatesiCharacter.Testing.Character.Ragdolls.RagdollProperties
                    {
                        asTrigger = false,
                        isKinematic = false,
                        rigidAngularDrag = 0.3f,
                        rigidDrag = 0.3f,
                        cdMode = CollisionDetectionMode.Continuous,
                        onlyCollider = false,
                    };
                    int _ragdollTotalWeight = 60;
                    ragdoller.ApplyRagdoll(_ragdollTotalWeight, _ragdollProperties);
                    characterComponent.CharacterMotionBase.RagdollMonitor.Initialize(characterComponent.CharacterMotionBase);
                    characterComponent.CharacterMotionBase.RagdollMonitor.EnableRagdoll();
                    characterComponent.CharacterMotionBase.RagdollMonitor.Force(characterComponent.CharacterMotionBase.Up * 2, ForceMode.VelocityChange);
                    characterComponent.CharacterMotionBase.RagdollMonitor.Force(characterComponent.CharacterMotionBase.Velocity * 1, ForceMode.VelocityChange);

                    characterComponent.CharacterMotionBase.Collider.enabled = false;

                    /*if (characterComponent.CharacterSO.Model != null)
                    {
                        var characterRagdoll = GameObject.Instantiate(
                            characterComponent.CharacterSO.Model, 
                            characterComponent.GameObject.transform.position, 
                            characterComponent.GameObject.transform.rotation
                        );
                        Debug.Log(characterComponent.GameObject.transform.position);
                        //var characterRagdollMotion = characterRagdoll.GetComponent<CharacterMotionBase>();
                        //characterRagdollMotion.Awake();
                        //characterRagdollMotion.SetPositionAndRotation(characterComponent.GameObject.transform.position, characterComponent.GameObject.transform.rotation);
                        //characterRagdollMotion.Collider.enabled = false;


                        var ragdoller = new InatesiCharacter.Testing.Character.Ragdolls.Ragdoller(characterRagdoll.transform, characterRagdoll.transform.forward);
                        var _ragdollProperties = new InatesiCharacter.Testing.Character.Ragdolls.RagdollProperties
                        {
                            asTrigger = false,
                            isKinematic = false,
                            rigidAngularDrag = 0.3f,
                            rigidDrag = 0.3f,
                            cdMode = CollisionDetectionMode.Continuous,
                            onlyCollider = false,
                        };
                        int _ragdollTotalWeight = 60;
                        ragdoller.ApplyRagdoll(_ragdollTotalWeight, _ragdollProperties);

                        if (characterRagdoll.TryGetComponent(out Animator component) ) 
                            Character.Ragdolls.Ragdoller.CopyBoneTransforms(component.GetBoneTransform(HumanBodyBones.Hips), characterComponent.CharacterMotionBase.RagdollMonitor.HipsBone);

                        //characterRagdollMotion.RagdollMonitor.Initialize(characterRagdollMotion);
                        //characterRagdollMotion.RagdollMonitor.CopyBoneTransforms(characterRagdollMotion.RagdollMonitor.HipsBone);
                        //characterRagdollMotion.RagdollMonitor.EnableRagdoll();
                        //characterRagdollMotion.RagdollMonitor.Force(characterRagdollMotion.Up * 2, ForceMode.VelocityChange);
                        //characterRagdollMotion.RagdollMonitor.Force(characterComponent.CharacterMotionBase.Velocity * 1, ForceMode.VelocityChange);
                    }*/




                    if (characterComponent.CharacterMotionBase.RagdollMonitor.IsActive)
                    {
                        //characterComponent.CharacterMotionBase.RagdollMonitor.EnableRagdoll();
                        //characterComponent.CharacterMotionBase.RagdollMonitor.Force(characterComponent.CharacterMotionBase.Velocity * 1, ForceMode.VelocityChange);
                    }
                    else
                    {
                        //characterComponent.CharacterMotionBase.Renderer.enabled = false;
                    }

                    if (characterComponent.CharacterSO.DissolveMaterial != null)
                    {
                        /*if (characterComponent.CharacterMotionBase.Renderer)
                        {
                            var texture = characterComponent.CharacterMotionBase.Renderer.material.mainTexture;
                            var color = characterComponent.CharacterMotionBase.Renderer.material.color;

                            Debug.Log(characterComponent.CharacterMotionBase.Renderer.materials.Length);

                            for (int i = 0; i < characterComponent.CharacterMotionBase.Renderer.materials.Length; i++)
                            {
                                characterComponent.CharacterMotionBase.Renderer.materials[i] = new Material(characterComponent.CharacterSO.DissolveMaterial);
                                characterComponent.CharacterMotionBase.Renderer.materials[i].SetTexture("_Texture_1", texture);
                                characterComponent.CharacterMotionBase.Renderer.materials[i].SetColor("_Color", color);
                            }

                            //characterComponent.CharacterMotionBase.Renderer.material = new Material(characterComponent.CharacterSO.DissolveMaterial);
                            //characterComponent.CharacterMotionBase.Renderer.material.SetTexture("_Texture_1", texture);
                            //characterComponent.CharacterMotionBase.Renderer.material.SetColor("_Color", color);
                        }*/

                        if (characterComponent.CharacterMotionBase.Renderers != null)
                        {
                            foreach (var renderer in characterComponent.CharacterMotionBase.Renderers)
                            {
                                var texture = renderer.material.mainTexture;
                                var color = renderer.material.color;

                                renderer.material = new Material(characterComponent.CharacterSO.DissolveMaterial);
                                renderer.material.SetTexture("_Texture_1", texture);
                                renderer.material.SetColor("_Color", color);
                                renderer.material.SetFloat("_Dissolve", 0);

                                /*for (int i = 0; i < renderer.materials.Length; i++)
                                {
                                    renderer.materials[i] = new Material(characterComponent.CharacterSO.DissolveMaterial);
                                    renderer.materials[i].SetTexture("_Texture_1", texture);
                                    renderer.materials[i].SetColor("_Color", color);
                                }*/
                            }

                            //characterComponent.CharacterMotionBase.Renderer.material = new Material(characterComponent.CharacterSO.DissolveMaterial);
                            //characterComponent.CharacterMotionBase.Renderer.material.SetTexture("_Texture_1", texture);
                            //characterComponent.CharacterMotionBase.Renderer.material.SetColor("_Color", color);
                        }
                    }

                    characterComponent.CharacterMotionBase.Collider.enabled = false;
                    characterComponent.CharacterMotionBase.Velocity = Vector3.zero;
                    characterComponent.CharacterMotionBase.InputDirection = Vector3.zero;
                    characterComponent.CharacterMotionBase.InputVector = Vector3.zero;
                }
            }


            foreach (var damageEntity in _DamageFilter)
            {
                ref var damageComponent = ref _DamagePool.Get(damageEntity);
                foreach (var characterEntity in _CharacterFilter)
                {
                    ref var characterComponent = ref _CharacterPool.Get(characterEntity);

                    if (characterComponent.GameObject == damageComponent.target)
                    {
                        characterComponent.Health -= (int)damageComponent.damage;
                        characterComponent.OnHealthChanged?.Invoke(characterComponent.Health);

                        if (characterComponent.Health > 0)
                        {
                            if (damageComponent.weaponType != null )
                            {
                                if (damageComponent.weaponType is ProjectileRPG || damageComponent.weaponType is WeaponBase)
                                {
                                    //characterComponent.CharacterMotionBase.AddForce(damageComponent.velocity);
                                }
                                
                            }
                            characterComponent.CharacterMotionBase.AddForce(damageComponent.velocity);
                            //if (characterComponent.CharacterMotionBase.AudioSource.isPlaying) characterComponent.CharacterMotionBase.AudioSource.Stop();

                        }
                        else
                        {
                            characterComponent.CharacterMotionBase.Velocity += damageComponent.velocity;
                        }

                        if (characterComponent.Dead == false && characterComponent.CharacterSO.AudioCharacter.HurtClips != null && characterComponent.CharacterSO.AudioCharacter.HurtClips.Length > 0)
                        {
                            var rnd = UnityEngine.Random.Range(0, characterComponent.CharacterSO.AudioCharacter.HurtClips.Length);
                            var audio = characterComponent.CharacterSO.AudioCharacter.HurtClips[rnd];
                            characterComponent.CharacterMotionBase.AudioSource.PlayOneShot(audio, characterComponent.CharacterSO.AudioCharacter.VolumeHurt);
                        }

                        if (characterComponent.CharacterSO.DamageVisualEffect != null && damageComponent.owner != null)
                        {
                            var visualEffect = GameObject.Instantiate(
                                characterComponent.CharacterSO.DamageVisualEffect,
                                damageComponent.position,
                                Quaternion.LookRotation((characterComponent.GameObject.transform.position - damageComponent.owner.transform.position).normalized, Vector3.up)
                            );
                            GameObject.Destroy(visualEffect.gameObject, 1f);
                        }
                    }

                    if (characterComponent.GameObject == damageComponent.target && characterComponent.Dead == true)
                    {
                        if (characterComponent.CharacterMotionBase.RagdollMonitor.IsActive)
                        {
                            //characterComponent.CharacterMotionBase.RagdollMonitor.EnableRagdoll();
                            characterComponent.CharacterMotionBase.RagdollMonitor.Force(damageComponent.velocity, ForceMode.VelocityChange);
                        }
                    }
                }
            }


            foreach (var character in _CharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(character);

                characterComponent.CharacterMotionBase.UpdateCharacterMethod();
                characterComponent.CharacterMotionBase.UpdateAnimator();
                characterComponent.CharacterMotionBase.CheckWater();
                characterComponent.CharacterMotionBase.UpdateFootstep();

                if (characterComponent.CharacterMotionBase.Renderer && characterComponent.CharacterSO.DissolveMaterial != null)
                {
                    float t = 1;
                    if (characterComponent.TimeOfDeath > t)
                    {   
                        var tt = characterComponent.TimeOfDeath - t / characterComponent.CharacterSO.CharacterConfig.TimeAfterDeath - t ;
                        var time = Mathf.Lerp(
                                                tt,
                                                .8f,
                                                tt
                                            );

                        for (int i = 0; i < characterComponent.CharacterMotionBase.Renderer.materials.Length; i++)
                        {
                            characterComponent.CharacterMotionBase.Renderer.materials[i].SetFloat("_Dissolve", time);
                        }

                        if (characterComponent.CharacterMotionBase.Renderers != null)
                        {
                            foreach (var renderer in characterComponent.CharacterMotionBase.Renderers)
                            {
                                for (int i = 0; i < renderer.materials.Length; i++)
                                {
                                    renderer.materials[i].SetFloat("_Dissolve", time);
                                }
                            }
                        }

                        //characterComponent.CharacterMotionBase.Renderer.material.SetFloat("_Dissolve", time);
                    }


                }
            }
        }

    }
}