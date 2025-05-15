using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character;
using InatesiCharacter.Testing.Character.Weapons;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.LeoEcs3.Shared;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Props;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.UI;
using Leopotam.EcsLite;
using System;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        private EcsFilter _PlayerFilter;
        private EcsFilter _CharacterDieFilter;
        private EcsPool<CharacterComponent> _CharacterPool;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsWorld _World;
        private EcsFilter _HitFilter;
        private EcsFilter _PlayerInputEvent;
        private EcsPool<PlayerInputEvent> _PlayerInputEventPool;


        public void Destroy(IEcsSystems systems)
        {
            GameSettings.PauseGameAction -= SetActiveInput;

            foreach (var playerEntity in _PlayerFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(playerEntity);
                ref var playerComponent = ref _PlayerPool.Get(playerEntity);

                characterComponent.OnHealthChanged -= playerComponent.HealthUI.Pain;
            }
        }

        public void Init(IEcsSystems systems)
        {
            _PlayerFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            GameSettings.PauseGameAction += SetActiveInput;
            _World = systems.GetWorld();
            _CharacterDieFilter = systems.GetWorld().Filter<CharacterDieEvent>().End();
            _HitFilter = systems.GetWorld().Filter<HitComponent>().End();
            _PlayerInputEvent = systems.GetWorld().Filter<PlayerInputEvent>().End();
            _PlayerInputEventPool = systems.GetWorld().GetPool<PlayerInputEvent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            foreach (var entity in systems.GetWorld().Filter<PlayerSpawnEvent>().End()) 
            {
                foreach (var playerEntity in _PlayerFilter)
                {
                    ref var characterComponent = ref _CharacterPool.Get(playerEntity);
                    ref var playerComponent = ref _PlayerPool.Get(playerEntity);

                    characterComponent.OnHealthChanged += playerComponent.HealthUI.Pain;
                    characterComponent.OnHealthChanged += playerComponent.HealthUI.UpdateUI;

                    playerComponent.HealthUI.UpdateUI(characterComponent.Health);
                    playerComponent.HealthUI.Reset();
                }

                SetActiveInput(false);
            }



            foreach (var entity in _PlayerInputEvent)
            {
                ref var playerInputEvent = ref _PlayerInputEventPool.Get(entity);
                SetActiveInput(playerInputEvent.enable);
            }


            var input =  Inatesi.Inputs.Input.GetVector("Move");
            var wishRun = Inatesi.Inputs.Input.Down("Run");
            var wishView = Inatesi.Inputs.Input.Pressed("View");
            var wishInventory = Inatesi.Inputs.Input.GetKeyDown(UnityEngine.InputSystem.Key.I);

            foreach (var playerEntity in _PlayerFilter)
            {
                ref var characterComponent = ref _CharacterPool.Get(playerEntity);
                ref var playerComponent = ref _PlayerPool.Get(playerEntity);

                input = playerComponent.moveInputEnabled ? Inatesi.Inputs.Input.GetVector("Move") : Vector2.zero;

                if (characterComponent.Dead == true) continue;


                if (wishInventory)
                {
                    playerComponent.InventoryUI.SetActivePanel(false);
                }

                foreach (var hitEntity in _HitFilter)
                {
                    ref var hitComponent = ref systems.GetWorld().GetPool<HitComponent>().Get(hitEntity);

                    if (hitComponent.other == characterComponent.GameObject)
                    {
                        if (hitComponent.data != null && hitComponent.data.Length > 0)
                        {
                            var propType = (PropType)hitComponent.data[0];
                            AudioClip audio = null;
                            var color = Color.green;

                            switch (propType)
                            {
                                case PropType.Health:
                                    var health = (float)hitComponent.data[1];
                                    audio = (AudioClip)hitComponent.data[2];
                                    characterComponent.Health = health;
                                    color = Color.green;
                                    playerComponent.HealthUI.SetColorPanel(new Color(color.r, color.g, color.b, .4f));
                                    if (audio) characterComponent.CharacterMotionBase.AudioSource.PlayOneShot(audio);
                                    break;

                                case PropType.Ammo:
                                    foreach (var item in characterComponent.InventoryInteraction2.InventoryContainer.InventoryItems)
                                    {
                                        if (item == null) continue;

                                        var weaponTypes = hitComponent.data[1] as Array;
                                        var name = (string)hitComponent.data[2];
                                        var ammo = (int)hitComponent.data[3];
                                        audio = (AudioClip)hitComponent.data[4];

                                        foreach (var weaponTypeItem in weaponTypes)
                                        {
                                            if (weaponTypeItem is not WeaponType) continue;

                                            var weaponType = (WeaponType)weaponTypeItem;

                                            if (name == item.CarriableObjectData.Name && weaponType == WeaponType.None)
                                            {
                                                //item.CarriableObjectData.Ammo += 100;
                                                item.CarriableObjectData.TotalAmmo += ammo;
                                            }
                                            else if (weaponType == item.CarriableObjectData.WeaponType)
                                            {
                                                item.CarriableObjectData.TotalAmmo += ammo;
                                            }

                                            
                                        }
                                    }

                                    color = Color.yellow;
                                    playerComponent.HealthUI.SetColorPanel(new Color(color.r, color.g, color.b, .4f));
                                    if (audio) characterComponent.CharacterMotionBase.AudioSource.PlayOneShot(audio);
                                    break;
                            }
                        }
                        

                        
                        playerComponent.HealthUI.UpdateUI(characterComponent.Health);
                    }
                }


                characterComponent.CharacterMotionBase.Move(input);
                characterComponent.CharacterMotionBase.LookSource.CameraMotion.InputEnabled = playerComponent.cameraInputEnabled;

                if (characterComponent.Crouched)
                {
                    characterComponent.CharacterMotionBase.SpeedMove = characterComponent.CharacterMotionBase.MoveConfig.CrouchingSpeed;
                }
                else if (wishRun && characterComponent.stamina < 4)
                {
                    characterComponent.stamina += Time.deltaTime;
                    characterComponent.CharacterMotionBase.SpeedMove = characterComponent.CharacterMotionBase.MoveConfig.RunSpeed;
                }
                else
                {
                    characterComponent.CharacterMotionBase.SpeedMove = characterComponent.CharacterMotionBase.MoveConfig.Speed;
                }

                if (!wishRun && characterComponent.stamina >= 0) characterComponent.stamina -= Time.deltaTime;


                if (wishView) 
                {
                    playerComponent.fpc = !playerComponent.fpc;

                    if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                        characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;

                    PlayerBuilder.SetCameraView(playerComponent.fpc, characterComponent.CharacterMotionBase);
                }


                playerComponent.HealthUI.ProcessUpdate();
            }

            foreach (var dieEntity in _CharacterDieFilter)
            {
                ref var dieComponent = ref systems.GetWorld().GetPool<CharacterDieEvent>().Get(dieEntity);

                if (_PlayerPool.Has(dieComponent.entityCharacter))
                {
                    ref var playerComponent = ref _PlayerPool.Get(dieComponent.entityCharacter);
                    Debug.Log("DIE PLAYER " + dieComponent.entityCharacter);

                    SetActiveInput(true);
                    _PlayerPool.Get(dieComponent.entityCharacter).HealthUI.Reset();
                    playerComponent.HealthUI.Pain(1);

                    if (_CharacterPool.Has(dieComponent.entityCharacter))
                    {
                        var characterComponent = _CharacterPool.Get(dieComponent.entityCharacter);
                        

                        playerComponent.fpc = false;

                        if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                            characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;

                        characterComponent.CharacterMotionBase.LookSource.CameraMotion.Follow = characterComponent.GameObject.transform;
                        PlayerBuilder.SetCameraView(playerComponent.fpc, characterComponent.CharacterMotionBase);
                    }
                    
                }
            }

            
        }

        private void SetActiveInput(bool active)
        {
            _PlayerFilter = _World.Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _PlayerPool = _World.GetPool<PlayerComponent>();
            _CharacterPool = _World.GetPool<CharacterComponent>();

            foreach (int entity in _PlayerFilter)
            {
                if (_CharacterPool.Has(entity))
                {
                    ref var characterComponent = ref _CharacterPool.Get(entity);
                    ref var playerComponent = ref _PlayerPool.Get(entity);

                    if (characterComponent.Dead)
                        continue;

                    playerComponent.moveInputEnabled = !active;
                    playerComponent.cameraInputEnabled = !active;
                    characterComponent.CharacterMotionBase.IsInputDisabled = active;
                    characterComponent.CharacterMotionBase.LookSource.CameraMotion.InputEnabled = !active;
                }
            }
        }




        public void OnEntityChanged(int entity)
        {

        }

        public void OnEntityCreated(int entity)
        {
            if (_World.GetPool<CharacterComponent>().Has(entity))
            {
                //Debug.Log(string.Format("{0} changed {1}", entity, 222));
            }
        }

        public void OnEntityDestroyed(int entity)
        {
            if (_World == null) return;
            

            //Debug.Log(string.Format("{0} destroy", entity));
        }

        public void OnFilterCreated(EcsFilter filter)
        {

        }

        public void OnWorldDestroyed(EcsWorld world)
        {

        }

        public void OnWorldResized(int newSize)
        {

        }
    }
}