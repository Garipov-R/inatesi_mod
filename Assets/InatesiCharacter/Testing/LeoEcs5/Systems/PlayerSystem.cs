using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class PlayerSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _CharacterFilter;
        private EcsFilter _PlayerCharacterFilter;
        private EcsFilter _PlayerFilter;
        private EcsFilter _BotFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;


        public void Init(IEcsSystems systems)
        {
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _BotFilter = systems.GetWorld().Filter<BotComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();

            var sharedData = systems.GetShared<SharedData>();
            if (sharedData.CharacterMotionBase != null )
            {
                var newCharacterEntity = systems.GetWorld().NewEntity();
                ref var characterComponent = ref _CharacterPool.Add(newCharacterEntity);

                var camera = GameObject.FindAnyObjectByType<LookSource>();



                ref var playerComponent = ref _PlayerPool.Add(newCharacterEntity);
                playerComponent.cameraMotion = camera.GetComponent<CameraMotion>();

                playerComponent.fpc = true;
                playerComponent.inputEnabled = true;

                characterComponent.CharacterSO = sharedData.CharacterSO;
                characterComponent.characterMotion = sharedData.CharacterMotionBase;
                characterComponent.characterMotion.LookSource = camera;


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
                    characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
                }

                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit1)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit2)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(1);
                if (Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.Digit3)) characterComponent.InventoryInteraction2.SetActiveInventoryItem(2);

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                    characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;













                characterComponent.characterMotion.UpdateCharacter();
                characterComponent.characterMotion.UpdateAnimator();
            }
        }
    }
}
