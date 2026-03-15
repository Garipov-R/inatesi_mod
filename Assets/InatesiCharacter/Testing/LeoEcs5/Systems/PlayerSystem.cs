using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class PlayerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsFilter _PlayerCharacterFilter;
        private EcsFilter _PlayerFilter;
        private EcsFilter _BotFilter;
        private EcsFilter _DamageFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsPool<DamageComponent> _DamagePool;

        private EcsWorld _ecsWorld;


        public void Init(IEcsSystems systems)
        {
            _ecsWorld = systems.GetWorld();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();

            var sharedData = systems.GetShared<SharedData>();
            if (sharedData.StartPlayerCharacterMotionBase != null )
            {
                var newCharacterEntity = systems.GetWorld().NewEntity();
                ref var characterComponent = ref _CharacterPool.Add(newCharacterEntity);

                var camera = GameObject.FindAnyObjectByType<LookSource>();



                ref var playerComponent = ref _PlayerPool.Add(newCharacterEntity);
                playerComponent.cameraMotion = camera.GetComponent<CameraMotion>();

                playerComponent.fpc = true;
                playerComponent.inputEnabled = true;
                playerComponent.uiDocument = sharedData.PlayerUIDocument;

                characterComponent.CharacterSO = sharedData.CharacterSO;
                characterComponent.characterMotion = sharedData.StartPlayerCharacterMotionBase;
                characterComponent.characterMotion.LookSource = camera;
                characterComponent.transform = sharedData.StartPlayerCharacterMotionBase.transform;
                characterComponent.gameObject = sharedData.StartPlayerCharacterMotionBase.gameObject;
                characterComponent.health = 30;
                characterComponent.characterMotion.OnLanded.AddListener( OnLanded);

                // inventory
                characterComponent.InventoryInteraction2 = new Character.InteractionSystem.InventoryInteraction2(characterComponent.characterMotion);
                characterComponent.InventoryInteraction2.InventoryContainer.Size = characterComponent.CharacterSO.StartWeaponSO.Weapons.Count;
                characterComponent.InventoryInteraction2.CharacterMotionBase = characterComponent.characterMotion;

                if (characterComponent.CharacterSO.StartWeaponSO.Weapons != null)
                {
                    foreach (var item in characterComponent.CharacterSO.StartWeaponSO.Weapons)
                    {
                        characterComponent.InventoryInteraction2.AddItem(item);
                    }
                }

                characterComponent.InventoryInteraction2.InitializeWeapons();
                characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase) characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;
            }



        }

        public void Run(IEcsSystems systems)
        {
            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(characterEntity);
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);

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
                        2f, 
                        LayerMask.NameToLayer("Everything"), 
                        QueryTriggerInteraction.Collide
                    );

                    if (cast)
                    {
                        if (hitInfo.transform.TryGetComponent(out CollisionEvent collisionEvent))
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




                if (Inatesi.Inputs.Input.AnyKeyDown())
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
                        characterComponent.characterMotion.AddForce(damageComponent.velocity);
                        var clips = systems.GetShared<SharedData>().CharacterSO.AudioCharacter.HurtClips;
                        characterComponent.characterMotion.AudioSource.PlayOneShot(
                            clips[UnityEngine.Random.Range(0, clips.Length - 1)]
                        );

                        characterComponent.health -= damageComponent.damage;
                    }
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
                ref var playerComponent = ref _PlayerPool.Get(characterEntity);

                if (characterComponent.characterMotion.Velocity.y <= characterComponent.CharacterSO.MoveConfig.FallDamageVelocity)
                {
                    ref var damageComponent = ref SendDamageEvent.Send(_ecsWorld);
                    damageComponent.damage = 10;
                    damageComponent.velocity = Vector3.up;
                    damageComponent.target = characterComponent.gameObject;

                    characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnLandedClip);
                }
            }
                
        }
    }
}
