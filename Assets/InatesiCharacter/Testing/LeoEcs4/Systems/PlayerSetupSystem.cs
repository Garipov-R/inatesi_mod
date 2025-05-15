using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter.MovementTypes;
using InatesiCharacter.Testing.Character;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.UI;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.UI;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerSetupSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _SpawnComponentFilter;
        private EcsFilter _CharacterFilter;
        private EcsFilter _PlayerFilter;
        private SharedData _sharedData;

        public void Init(IEcsSystems systems)
        {
            _SpawnComponentFilter = systems.GetWorld().Filter<SpawnComponentEvent>().End();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Exc<PlayerComponent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _sharedData = systems.GetShared<SharedData>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _PlayerFilter)
            {
                //Debug.Log(playerEntity + " player");
                return;
            }

            foreach (var spawnComponentEntity in _SpawnComponentFilter) 
            {
                ref var spawnComponent = ref systems.GetWorld().GetPool<SpawnComponentEvent>().Get(spawnComponentEntity);

                if (spawnComponent.isPlayer == false)
                    continue;

                if (spawnComponent.data is not CharacterSO)
                    continue;

                foreach (var characterEntity in _CharacterFilter)
                {
                    if (characterEntity != spawnComponent.entity) continue;

                    ref var playerComponent = ref systems.GetWorld().GetPool<PlayerComponent>().Add(characterEntity);

                    ref var characterComponent = ref systems.GetWorld().GetPool<CharacterComponent>().Get(characterEntity);

                    PlayerBuilder.Build(characterComponent.CharacterMotionBase);

                    characterComponent.CharacterMotionBase.IsInputDisabled = false;
                    characterComponent.CharacterMotionBase.SetMovementType(new Combat());
                    characterComponent.GameObject.layer = LayerMask.NameToLayer("Player");
                    characterComponent.GameObject.name += "_PLAYER";
                    characterComponent.CharacterMotionBase.LookSource.CameraMotion = characterComponent.CharacterMotionBase.LookSource.GameObject.GetComponent<CameraMotion>();


                    foreach (var child in characterComponent.GameObject.GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("Player");
                    }


                    characterComponent.InventoryInteraction2 = new Character.InteractionSystem.InventoryInteraction2(characterComponent.CharacterMotionBase);
                    characterComponent.InventoryInteraction2.CharacterMotionBase = characterComponent.CharacterMotionBase;

                    if (characterComponent.CharacterSO.StartWeaponSO.Weapons != null)
                    {
                        foreach (var item in characterComponent.CharacterSO.StartWeaponSO.Weapons)
                        {
                            characterComponent.InventoryInteraction2.AddItem(item);
                        }
                    }

                    characterComponent.InventoryInteraction2.InitializeWeapons();
                    characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);

                    playerComponent.InventoryUI = new InventoryUI(characterComponent.InventoryInteraction2.InventoryContainer);
                    playerComponent.HealthUI = new();
                    playerComponent.fpc = true;
                    if (characterComponent.InventoryInteraction2.CurrentWeaponBase) characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;

                    var playerSpawn = systems.GetWorld().NewEntity();
                    var playerSpawnPool = systems.GetWorld().GetPool<PlayerSpawnEvent>().Add(playerSpawn);
                    //characterComponent.OnHealthChange?.Invoke(characterComponent.Health);

                    PlayerBuilder.SetCameraView(playerComponent.fpc, characterComponent.CharacterMotionBase);

                    break;
                }
            }
        }
    }
}