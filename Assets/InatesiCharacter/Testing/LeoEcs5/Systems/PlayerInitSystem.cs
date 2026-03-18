using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
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
    public class PlayerInitSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerInitFilter;
        private EcsFilter _PlayerCharacterFilter;
        private EcsFilter _PlayerFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;

        public void Init(IEcsSystems systems)
        {
            _PlayerInitFilter = systems.GetWorld().Filter<PlayerInitEvent>().End();
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerInitEntity in _PlayerInitFilter)
            {
                var sharedData = systems.GetShared<SharedData>();
                if (sharedData.StartPlayerCharacterMotionBase != null)
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
                    //characterComponent.characterMotion.OnLanded.AddListener(OnLanded);
                    playerComponent.cameraMotion.Follow = characterComponent.transform;
                    playerComponent.gameObject = characterComponent.characterMotion.gameObject;

                    // inventory
                    characterComponent.InventoryInteraction2 = new Character.InteractionSystem.InventoryInteraction2(characterComponent.characterMotion);
                    characterComponent.InventoryInteraction2.InventoryContainer.Size = 3;
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
                    characterComponent.InventoryInteraction2.EnableCurrentWeapon();

                    if (characterComponent.InventoryInteraction2.CurrentWeaponBase) characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;

                }
            }
        }
    }
}
