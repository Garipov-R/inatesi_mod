using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs5.Components;
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



            UpdateUI();
        }

        public void Run(IEcsSystems systems)
        {
            if (G.IsPause) return;


            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);


                foreach (var playerInitEntity in _PlayerInitFilter)
                {
                    characterComponent.characterMotion.OnLanded.RemoveAllListeners();
                    characterComponent.characterMotion.OnLanded.AddListener(OnLanded);
                }


                if (playerComponent.inputEnabled == false)
                    continue;

                var input = Inatesi.Inputs.Input.GetVector("Move");
                var wishJump = Inatesi.Inputs.Input.Pressed("Jump");
                var wishUse = Inatesi.Inputs.Input.Pressed("Use");
                var wishAttack = Inatesi.Inputs.Input.Pressed("Attack");

                //Debug.Log(input);

                characterComponent.characterMotion.Move(input);


                var zoomInput = Inatesi.Inputs.Input.GetVector("Scroll");
                if (Mathf.Abs( zoomInput.y) > 0)
                {
                    playerComponent.cameraMotion.ZoomAmount = zoomInput.y;
                }

                if (wishJump == true && characterComponent.characterMotion.OnGrounded == true)
                {
                    characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnJumpClip);
                    characterComponent.characterMotion.AddForce(Vector3.up * characterComponent.characterMotion.MoveConfig.JumpForce);
                }

                if (wishUse)
                {
                    var startPoint = characterComponent.characterMotion.transform.position + characterComponent.characterMotion.Up * characterComponent.characterMotion.Height;

                    var cast = Physics.Raycast(
                        startPoint, 
                        characterComponent.characterMotion.LookSource.LookDirection(), 
                        out RaycastHit hitInfo, 
                        3f, 
                        LayerMask.NameToLayer("Everything"), 
                        QueryTriggerInteraction.Collide
                    );

                    if (cast)
                    {
                        if (hitInfo.transform.TryGetComponent(out InatesiCharacter.Testing.Character.InteractionSystem.CollisionEvent collisionEvent))
                        {
                            collisionEvent.Use();
                        }
                    }
                }

                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit1)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit2)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(1);
                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit3)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(2);

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                    characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;




                foreach (var entityDamage in _DamageFilter)
                {
                    ref var damageComponent = ref _DamagePool.Get(entityDamage);

                    if (damageComponent.target == characterComponent.gameObject)
                    {
                        characterComponent.characterMotion.AddForce(damageComponent.velocity);
                        var clips = systems.GetShared<SharedData>().CharacterSO.AudioCharacter.HurtClips;
                        characterComponent.characterMotion.AudioSource.PlayOneShot(
                            clips[UnityEngine.Random.Range(0, clips.Length - 1)]
                        );

                        if (damageComponent.damage > 999)
                        {
                            characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnLandedClip);
                        }

                        characterComponent.health -= damageComponent.damage;

                        if (characterComponent.health <= 0)
                        {
                            //playerComponent.cameraMotion.InputEnabled = false;
                            playerComponent.cameraMotion.Follow = null;
                            playerComponent.cameraMotion.transform.position = characterComponent.transform.position + characterComponent.transform.up * .5f;
                            playerComponent.inputEnabled = false;
                            characterComponent.characterMotion.Velocity = Vector3.zero;
                            characterComponent.characterMotion.InputVector = Vector3.zero;
                            characterComponent.characterMotion.InputDirection = Vector3.zero;
                            characterComponent.InventoryInteraction2.DisableCurrentWeapon();
                        }

                        UpdateUI();
                    }
                }

                foreach (var collicionEventEntity in _CollisionEventFilter)
                {
                    ref var collisionEvent = ref _CollisionEventPool.Get(collicionEventEntity);
                    

                    if (collisionEvent.collideGameObject == playerComponent.gameObject)
                    {
                        _sharedData.GameLogic.OnPlayerCollision(collisionEvent);

                        if (collisionEvent.gameObject.TryGetComponent(out CarriableObject item))
                        {
                            var i= characterComponent.InventoryInteraction2.AddItem(item.ItemScriptableObject);
                            characterComponent.InventoryInteraction2.SetActiveInventoryItem(i.SlotIndex);
                        }
                    }

                    
                }



                if (Inatesi.Inputs.Input.AnyKeyDown())
                {
                    UpdateUI();
                }

                characterComponent.characterMotion.UpdateCharacter();
                characterComponent.characterMotion.UpdateAnimator();
                characterComponent.characterMotion.UpdateFootstep();
            }
        }

        private void OnLanded()
        {
            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);

                if (characterComponent.characterMotion.Velocity.y <= characterComponent.CharacterSO.MoveConfig.FallDamageVelocity)
                {

                    ref var damageComponent = ref SendDamageEvent.Send(_ecsWorld);
                    damageComponent.damage = Mathf.Abs( characterComponent.characterMotion.Velocity.y);
                    damageComponent.velocity = Vector3.up;
                    damageComponent.target = characterComponent.gameObject;

                    characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnLandedClip);
                }
            }
                
        }

        private void UpdateUI()
        {
            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase != null)
                {
                    string ammoText = string.Empty;
                    if (characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.WeaponType != Character.Weapons.WeaponType.None)
                    {
                        ammoText =
                            $"{characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.Ammo} /" +
                            $"{characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.TotalAmmo} ";
                    }
                    else
                    {
                        ammoText = "hell";
                    }

                    playerComponent.uiDocument.rootVisualElement.Q<Label>("ammo").text = ammoText;

                }





                foreach (var entityDamage in _DamageFilter)
                {
                    ref var damageComponent = ref _DamagePool.Get(entityDamage);

                    if (damageComponent.target == characterComponent.gameObject)
                    {
                        _sharedData.SimplePlayerUI.ScreenEffect(false);
                    }
                }


                _sharedData.SimplePlayerUI.HealthLabel.text = characterComponent.health > 0 ? characterComponent.health.ToString("00") : "defunct";
            }
        }
    }
}
