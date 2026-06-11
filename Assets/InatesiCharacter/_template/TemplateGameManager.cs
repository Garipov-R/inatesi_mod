using GameToolkit.Localization;
using Inatesi.Game;
using InatesiCharacter._template;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.SuperCharacter.MovementTypes;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UIElements;
using Input = Inatesi.Inputs.Input;


namespace InatesiCharacter.Template
{
    public class TemplateGameManager : GameLogicBase
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private PlayerUI _PlayerUI;

        private float _DeadTimeSince;
        private bool _aim;

        [Zenject.Inject]
        private void TestInject(StartEcs startEcs)
        {
            _StartEcs = startEcs;
            Debug.Log(123);
        }

        private void Start()
        {
            G.SetVisibleCursor(false);

            Localization.Instance.CurrentLanguage = Language.Russian;

            Invoke(nameof(StartGame), .2f);
            //StartGame();
        }

        private void Update()
        {
            if (Input.Pressed("MainMenu"))
            {
                G.SetPause(!G.IsPause);
                G.SetVisibleCursor(G.IsPause);
                _mainMenuUI.SetRootPanel(G.IsPause);
            }

            if (PlayerAlive == false)
                return;
            

            //if (Input.Down("Secondary Attack"))
            if (Input.GetKeyDown(UnityEngine.InputSystem.Key.Q))
            {
                ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                //playerComponent.riggingTest.Target.position = playerComponent.cameraMotion.transform.forward * 15f;
                //characterComponent.characterMotion.ActiveMovementType = new Combat();
                //characterComponent.characterMotion.ActiveMovementType.Initialize

                _aim = !_aim;
                playerComponent.riggingTest.RightHandRig.weight = _aim  ? 1 : 0;
                characterComponent.characterMotion.AnimatorMonitor.Animator.SetLayerWeight(1, _aim ? 1 : 0);
                characterComponent.characterMotion.AnimatorMonitor.SetSlot0((int)characterComponent.InventoryInteraction2.InventoryContainer.ActiveItem.ItemScriptableObject.WeaponTypeAnimation);
            }

            if (_aim)
            {
                if (ECSHelper.Has<PlayerComponent>(_StartEcs.EcsWorld))
                {
                    ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                    playerComponent.riggingTest.Target.position = playerComponent.cameraMotion.transform.position + playerComponent.cameraMotion.transform.forward * 5;
                }
            }
            

            var wishView = Inatesi.Inputs.Input.Pressed("View");
            if (wishView)
            {
                SetFPSCamera(!FPS);
                ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                characterComponent.characterMotion.FirstPersonSettings.HideObjects(!FPS);
            }
        }

        public override void OnPlayerUseItem(bool use)
        {

        }

        public override void StartGame()
        {
            ECSHelper.Create<PlayerInitEvent>(_StartEcs.EcsWorld);
        }

        public override void OnPlayerSpawn()
        {
            SetFPSCamera(false);

            if (_PlayerUI == null)
                return;
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());

            _PlayerUI.UpdateHealthUI((int)characterComponent.health);
        }

        public override void OnPlayerPickupItem(ItemScriptableObject itemScriptableObject)
        {

        }

        public override void OnPlayerSelectItem(ItemScriptableObject itemScriptableObject)
        {
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);


            _aim = itemScriptableObject != null;
            if (itemScriptableObject.WeaponTypeAnimation != WeaponTypeAnimation.none)
            {
                playerComponent.riggingTest.RightHandRig.weight = _aim ? 1 : 0;
                characterComponent.characterMotion.AnimatorMonitor.Animator.SetLayerWeight(1, _aim ? 1 : 0);
            }
            else
            {
                characterComponent.characterMotion.AnimatorMonitor.Animator.SetLayerWeight(1, 0);
                playerComponent.riggingTest.RightHandRig.weight = 0;
            }
            characterComponent.characterMotion.AnimatorMonitor.SetSlot0((int)itemScriptableObject.WeaponTypeAnimation);
        }

        public override void OnPlayerDamage(DamageComponent damageComponent)
        {
            if (_PlayerUI == null)
                return;

            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());

            _PlayerUI.UpdateHealthUI((int)characterComponent.health);
        }

        public override void OnPlayerLanded()
        {
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);

            if (characterComponent.characterMotion.Velocity.y <= characterComponent.CharacterSO.MoveConfig.FallDamageVelocity)
            {

                ref var damageComponent = ref SendDamageEvent.Send(_StartEcs.EcsWorld);
                damageComponent.damage = Mathf.Abs(characterComponent.characterMotion.Velocity.y) / 2;
                damageComponent.velocity = Vector3.up;
                damageComponent.target = characterComponent.gameObject;
                damageComponent.hit = new RaycastHit
                {
                    point = characterComponent.transform.position + characterComponent.transform.up * 2f
                };
                damageComponent.ray = new Ray(characterComponent.transform.position, characterComponent.transform.forward);

                characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnLandedClip);
            }

            characterComponent.characterMotion.AnimatorMonitor.SetAbilityID((int)TransitionAnimationState.Idle);
        }
    }
}

