using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.UI;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerPropPanelSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerFilter;
        private EcsPool<PlayerComponent> _PlayerPool;
        private EcsFilter _DieFilter;
        private EcsPool<CharacterDieEvent> _CharacterDieEventPool;
        private EcsPool<CharacterComponent> _CharacterPool;

        private bool _activePanel = false;


        public void Init(IEcsSystems systems)
        {
            _PlayerFilter = systems.GetWorld().Filter<PlayerComponent>().End();
            _PlayerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _DieFilter = systems.GetWorld().Filter<CharacterDieEvent>().End();
            _CharacterDieEventPool = systems.GetWorld().GetPool<CharacterDieEvent>();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            var wishPropPanel = Inatesi.Inputs.Input.Down("WishPropPanel");
            var wishPropPanelReleased = Inatesi.Inputs.Input.Released("WishPropPanel");
            var wishPropPanelPressed = Inatesi.Inputs.Input.Pressed("WishPropPanel");
            var wishMoveCamera = Inatesi.Inputs.Input.Down("Secondary Attack");
            var wishMoveCameraReleased = Inatesi.Inputs.Input.Released("Secondary Attack");

            if (GameSettings.IsPause == true)
            {
                wishPropPanel = false;
            }

            foreach (var entity in _PlayerFilter)
            {
                ref var playerComponent = ref _PlayerPool.Get(entity);
                if (_CharacterPool.Get(entity).Dead) wishPropPanelPressed = false;
            }

            foreach (var entity in _DieFilter)
            {
                ref var c = ref _CharacterDieEventPool.Get(entity);

                if (_PlayerPool.Has(c.entityCharacter) == false) continue;

                if (_activePanel == false)
                {
                    wishPropPanelPressed = true;
                }
            }

            if (!_activePanel)
            {
                foreach (var entity in _PlayerFilter)
                {
                    ref var playerComponent = ref _PlayerPool.Get(entity);

                    if (wishMoveCamera)
                    {
                        playerComponent.moveInputEnabled = true;
                        playerComponent.cameraInputEnabled = true;
                    }

                    if (wishMoveCameraReleased)
                    {
                        playerComponent.moveInputEnabled = true;
                        playerComponent.cameraInputEnabled = false;
                    }
                }
            }
            

            if (wishPropPanelPressed)
            {
                _activePanel = !_activePanel;
                RootUI.Instance.PropPanelUIDocument.rootVisualElement.Q<VisualElement>().visible = !_activePanel;

                foreach (var entity in _PlayerFilter)
                {
                    ref var playerComponent = ref _PlayerPool.Get(entity);
                    playerComponent.moveInputEnabled = true;
                    playerComponent.cameraInputEnabled = _activePanel;
                }

                //SetActiveInput(!statt);

                //var newEntity = systems.GetWorld().NewEntity();
                //ref var playerInputEvent = ref systems.GetWorld().GetPool<PlayerInputEvent>().Add(newEntity);
                //playerInputEvent.enable = !statt;


                UnityEngine.Cursor.visible = !_activePanel;
                UnityEngine.Cursor.lockState = !_activePanel ? CursorLockMode.Confined : CursorLockMode.Locked;
            }
        }
    }
}