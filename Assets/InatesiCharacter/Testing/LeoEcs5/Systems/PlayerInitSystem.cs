using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.AnimationRiging;
using InatesiCharacter.Testing.Character.Bot3;
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
                ref var playerInitEventComponent = ref systems.GetWorld().GetPool<PlayerInitEvent>().Get(playerInitEntity);
                var sharedData = systems.GetShared<SharedData>();
                if (sharedData.StartPlayerCharacterMotionBase != null)
                {
                    var newCharacterEntity = systems.GetWorld().NewEntity();
                    playerInitEventComponent.entity = newCharacterEntity;



                    // character component setup ================================
                    ref var characterComponent = ref _CharacterPool.Add(newCharacterEntity);
                    characterComponent.CharacterSO = sharedData.CharacterSO;
                    characterComponent.characterMotion = sharedData.StartPlayerCharacterMotionBase;
                    characterComponent.transform = sharedData.StartPlayerCharacterMotionBase.transform;
                    characterComponent.gameObject = sharedData.StartPlayerCharacterMotionBase.gameObject;
                    characterComponent.health = 100;
                    if (characterComponent.gameObject.TryGetComponent(out EntityFlagMono entityFlagMono))
                    {
                        characterComponent.entityFlag = entityFlagMono.EntityFlag;
                    }
                    // ================================================================


                    // player component setup ================================
                    var camera = GameObject.FindAnyObjectByType<LookSource>();
                    ref var playerComponent = ref _PlayerPool.Add(newCharacterEntity);
                    playerComponent.cameraMotion = camera.GetComponent<CameraMotion>();
#if UNITY_ANDROID
                    playerComponent.cameraMotion.NewInputSystem = true;
#endif
                    playerComponent.fpc = true;
                    playerComponent.inputEnabled = true;
                    playerComponent.uiDocument = sharedData.PlayerUIDocument;
                    playerComponent.cameraMotion.Follow = characterComponent.transform;
                    playerComponent.gameObject = characterComponent.characterMotion.gameObject;
                    characterComponent.characterMotion.LookSource = camera;
                    playerComponent.riggingTest = characterComponent.gameObject.GetComponent<RiggingTest>();
                    // end ===============================


                    // <<inventory>> ===================================
                    characterComponent.InventoryInteraction2 = new Character.InteractionSystem.InventoryInteraction2(characterComponent.characterMotion);
                    characterComponent.InventoryInteraction2.InventoryContainer.Size = 4;
                    characterComponent.InventoryInteraction2.CharacterMotionBase = characterComponent.characterMotion;
                    characterComponent.InventoryInteraction2.EcsWorld = systems.GetWorld();

                    if (characterComponent.CharacterSO.StartWeaponSO.Weapons != null)
                    {
                        foreach (var item in characterComponent.CharacterSO.StartWeaponSO.Weapons)
                        {
                            characterComponent.InventoryInteraction2.AddItem(item);
                        }
                    }

                    //characterComponent.InventoryInteraction2.InitializeWeapons();
                    characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
                    characterComponent.InventoryInteraction2.EnableCurrentWeapon();

                    if (characterComponent.InventoryInteraction2.CurrentWeaponBase) characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;
                    // end <<inventory>> ================================




                    // FIX
                    if (characterComponent.gameObject.GetComponentInChildren<Animator>())
                    {

                        Debug.Log(characterComponent.gameObject.transform.GetChild(0).gameObject.name);
                        characterComponent.characterMotion.AnimatorMonitor.Initialize(characterComponent.gameObject.transform.GetChild(0).gameObject);
                        characterComponent.characterMotion.AnimatorMonitor.SetAvatar(characterComponent.gameObject.transform.GetChild(0).GetComponentInChildren<Animator>().avatar);
                    }

                }
            }
        }
    }
}
