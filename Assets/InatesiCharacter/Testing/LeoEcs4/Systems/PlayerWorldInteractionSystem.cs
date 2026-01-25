using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.Character.Weapons;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.UI;
using Leopotam.EcsLite;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerWorldInteractionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<PlayerComponent> _playerPool;
        private EcsPool<CharacterComponent> _characterPool;
        private EcsFilter _SpawnComponentFilter;
        private EcsFilter _PlayerFilter;

        // shiiiiiiiiiiiiiiiiiiiiiiit
        Light _flashlight;

        public void Init(IEcsSystems systems)
        {
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _playerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _characterPool = systems.GetWorld().GetPool<CharacterComponent>(); 
            _SpawnComponentFilter = systems.GetWorld().Filter<SpawnComponentEvent>().End();
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;
            if (G.IsPause) return;

            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _characterPool.Get(characterEntity);
                ref var playerComponent = ref _playerPool.Get(characterEntity);

                if (characterComponent.Dead == true) continue;

                if (playerComponent.canAttack == false) continue;


                /* ======================================================== */
                /* ======================================================== */
                /* ======================================================== */
                if (Inatesi.Inputs.Input.Pressed("Use"))
                {
                    RaycastHit hit;
                    var isHit = Physics.Raycast(
                        characterComponent.CharacterMotionBase.LookSource.LookPosition(),
                        characterComponent.CharacterMotionBase.LookSource.Transform.forward,
                        out hit,
                        1f + Vector3.Distance(characterComponent.CharacterMotionBase.transform.position, characterComponent.CharacterMotionBase.LookSource.Transform.position),
                        characterComponent.CharacterMotionBase.RaycastLayer,
                        QueryTriggerInteraction.Collide
                    );

                    if (isHit == true)
                    {
                        if (hit.transform.TryGetComponent(out Testing.Character.InteractionSystem.CarriableObject component))
                        {
                            var canAdd = characterComponent.InventoryInteraction2.AddItem(component);
                            //characterComponent.InventoryInteraction2.SetActiveInventoryItem(characterComponent.InventoryInteraction2.InventoryContainer.ActiveSlotIndex);

                            if (canAdd)
                                GameObject.Destroy(component.gameObject);
                        }
                        else if (hit.transform.TryGetComponent(out TriggerTouch triggerTouch))
                        {
                            triggerTouch.OnTouch.Invoke();
                        }
                        else if (hit.transform.TryGetComponent(out CollisionEvent collisionEvent))
                        {
                            collisionEvent.Use();
                        }
                    }
                }
                /* ======================================================== */
                /* ======================================================== */
                /* ======================================================== */


                if (Mathf.Abs(Inatesi.Inputs.Input.GetVector("Zoom").y) > 0 && !Inatesi.Inputs.Input.Down("Attack") && playerComponent.canAttack == true)
                {
                    var index = Inatesi.Inputs.Input.GetVector("Zoom").y < 0 ? 1 : -1;
                    index = characterComponent.InventoryInteraction2.InventoryContainer.ActiveSlotIndex + index;

                    if (index < 0)
                    {
                        index = characterComponent.InventoryInteraction2.InventoryContainer.Size - 1;
                    }
                    else if (index >= characterComponent.InventoryInteraction2.InventoryContainer.Size)
                    {
                        index = 0;
                    }
                    index = Mathf.Clamp(index, 0, characterComponent.InventoryInteraction2.InventoryContainer.Size - 1);
                    //SetActiveInventoryItem2(_inventoryInteraction.InventoryContainer.ActiveSlotIndex + index);
                    characterComponent.InventoryInteraction2.SetActiveInventoryItem(index);
                    UpdateWeaponAmmoUI();
                }

                if (Inatesi.Inputs.Input.Pressed("slot1")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
                if (Inatesi.Inputs.Input.Pressed("slot2")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(1);
                if (Inatesi.Inputs.Input.Pressed("slot3")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(2);
                if (Inatesi.Inputs.Input.Pressed("slot4")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(3);
                if (Inatesi.Inputs.Input.Pressed("slot5")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(4);
                if (Inatesi.Inputs.Input.Pressed("slot6")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(5);
                if (Inatesi.Inputs.Input.Pressed("Drop")) 
                {
                    characterComponent.InventoryInteraction2.Drop();
                    characterComponent.InventoryInteraction2.SetActiveInventoryItem(1);
                }

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                    characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;

                /*if (characterComponent.InventoryInteraction2 != null)
                    characterComponent.InventoryInteraction2.UpdateEffectTick();*/

                bool attack = Inatesi.Inputs.Input.Down("attack");
                bool secondaryAttack = Inatesi.Inputs.Input.Down("Secondary Attack");

                if (Inatesi.Inputs.Input.AnyKeyDown())
                {
                    UpdateWeaponAmmoUI();
                }

                if (attack || secondaryAttack)
                {
                    UpdateWeaponAmmoUI();
                }

                // =============================================================================================================
                // =============================================================================================================
                // =============================================================================================================
                if (Inatesi.Inputs.Input.Pressed("flashlight"))
                {
                    if (playerComponent.fpc == true)
                    {
                        foreach (var child in characterComponent.GameObject.GetComponentsInChildren<Transform>())
                        {
                            if (child.gameObject.name == "flashlight")
                            {
                                child.SetParent(characterComponent.CharacterMotionBase.LookSource.Transform);
                                child.localEulerAngles = Vector3.zero;
                                child.localPosition = Vector3.zero;

                                _flashlight = child.gameObject.GetComponent<Light>();
                            }
                        }
                    }
                    else
                    {
                        foreach (var child in characterComponent.CharacterMotionBase.LookSource.GameObject.GetComponentsInChildren<Transform>())
                        {
                            if (child.gameObject.name == "flashlight")
                            {
                                child.SetParent(characterComponent.GameObject.transform);
                                child.localEulerAngles = Vector3.zero;
                                child.localPosition = new Vector3(0, characterComponent.CharacterMotionBase.Height - characterComponent.CharacterMotionBase.Radius / 2, characterComponent.CharacterMotionBase.Radius / 2) ;

                                _flashlight = child.gameObject.GetComponent<Light>();
                            }
                        }
                    }

                    if (_flashlight)
                    {
                        _flashlight.enabled = !_flashlight.enabled;
                    }

                    if (systems.GetShared<SharedData>().PlayerSettingsSO.FlashlightClip)
                        characterComponent.CharacterMotionBase.AudioSource.PlayOneShot(systems.GetShared<SharedData>().PlayerSettingsSO.FlashlightClip);
                }
                // =============================================================================================================
                // =============================================================================================================
                // =============================================================================================================

                break;
            }
        }

        private void UpdateWeaponAmmoUI()
        {
            string text = string.Empty;

            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _characterPool.Get(characterEntity);

                if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                { 
                    if(characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.WeaponType == WeaponType.None)
                    {
                        text = string.Empty;
                    }
                    else
                    {
                        text =
                            characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.Ammo.ToString() + "/" +
                            characterComponent.InventoryInteraction2.CurrentWeaponBase.CarriableObjectData.TotalAmmo.ToString();
                    }
                    
                }
                else
                {
                    text = string.Empty;
                }
            }

            if (RootUI.Instance == null) return;
            RootUI.Instance.UiDocument.rootVisualElement.Q<Label>("ammo").text = text;
 
            RootUI.Instance.UiDocument.rootVisualElement.Q<Label>("ammo-text").style.display = text == string.Empty ? DisplayStyle.None : DisplayStyle.Flex;
            RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("ammo-panel").style.display = text == string.Empty ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}