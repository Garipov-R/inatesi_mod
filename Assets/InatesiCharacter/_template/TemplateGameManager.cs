using GameToolkit.Localization;
using Inatesi.Game;
using InatesiCharacter._template;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.SuperCharacter.MovementTypes;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Input = Inatesi.Inputs.Input;


namespace InatesiCharacter.Template
{
    public class TemplateGameManager : GameLogicBase
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private PlayerUI _PlayerUI;
        [SerializeField] private FogSettingSO _underwaterFogSettingSO;
        [SerializeField] private Volume _DamageVolume;
        [SerializeField] private WaypointUITest _waypointUITest;

        private float _DeadTimeSince;
        private float _aimTimeSince;
        private bool _aim;
        private bool _aimWeapon;

        [Zenject.Inject]
        private void TestInject(StartEcs startEcs)
        {
            _StartEcs = startEcs;
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

            InputUpdate();




            if (_aim == true && _aimWeapon == false)
            {
                _aimTimeSince -= Time.deltaTime;

                if (_aimTimeSince < 0)
                {
                    SetAimAnimation(false);
                }
            }


            if (FPS == false)
            {
                CameraMotion.ZoomAmount = Mathf.Lerp(CameraMotion.ZoomAmount, _aimWeapon ? 1f : 3, Time.deltaTime * 10f);
            }



            if (ECSHelper.Has<PlayerComponent>(_StartEcs.EcsWorld))
            {
                ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());

                if (playerComponent.riggingTest != null)
                {
                    playerComponent.riggingTest.Target.position = playerComponent.cameraMotion.transform.position + playerComponent.cameraMotion.transform.forward * 5;

                }

                characterComponent.characterMotion.AnimatorMonitor.Animator.SetLayerWeight(
                    1, 
                    Mathf.Lerp(characterComponent.characterMotion.AnimatorMonitor.Animator.GetLayerWeight(1), _aim ? 1 : 0, Time.deltaTime * 10f) 
                );

            }


            if (_DamageVolume)
            {
                _DamageVolume.weight = Mathf.Lerp(_DamageVolume.weight, 0, Time.deltaTime * 2f);
            }


            if (_waypointUITest)
            {
                ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                
                if (ECSHelper.Has<BotComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<BotComponent>().End()))
                {
                    ref var botComponent = ref ECSHelper.Get<BotComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<BotComponent>().End());

                    _waypointUITest.Camera = playerComponent.cameraMotion.Camera;
                    _waypointUITest.Target = botComponent.gameObject.transform;
                }

                _waypointUITest.Player = playerComponent.gameObject.transform;
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

            SetAimAnimation(false);
            _aimWeapon = false;
        }

        public override void OnPlayerDamage(DamageComponent damageComponent)
        {
            if (_PlayerUI == null)
                return;

            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                
            _PlayerUI.UpdateHealthUI((int)characterComponent.health);

            if (_DamageVolume != null)
            {
                _DamageVolume.weight = 1;
            }

            if (_waypointUITest != null)
            {
                _waypointUITest.RegisterHit(damageComponent.ray.origin);
            }
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

            characterComponent.characterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Idle);
        }

        public override void OnPlayerAimEvent()
        {
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
            ref var playerCombatEvent = ref ECSHelper.Get<CombatEvent>(_StartEcs.EcsWorld);

            _aim = playerCombatEvent.aim;

            if (playerCombatEvent.aim)
            {

            }
            else
            {
                _aimTimeSince = 0;
            }

            SetAimAnimation(playerCombatEvent.aim);

            _aimWeapon = playerCombatEvent.aim;
        }

        public override void OnPlayerShoot()
        {
            SetAimAnimation(true);
        }

        private void SetAimAnimation(bool state)
        {
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);

            _aim = state;
            _aimTimeSince = 2;
            characterComponent.characterMotion.ActiveMovementType = _aim ? new Combat() : new Adventure();
            characterComponent.characterMotion.ActiveMovementType.Initialize(characterComponent.characterMotion);

            if (playerComponent.riggingTest)
            {
                playerComponent.riggingTest.RightHandRig.weight = _aim ? 1 : 0;
            }

            //characterComponent.characterMotion.AnimatorMonitor.Animator.SetLayerWeight(1, _aim ? 1 : 0);
            characterComponent.characterMotion.AnimatorMonitor.SetSlot0((int)characterComponent.InventoryInteraction2.InventoryContainer.ActiveItem.ItemScriptableObject.WeaponTypeAnimation);
        }

        public override void OnPlayerCollision(CollisionComponentEvent collisionEvent)
        {
            
        }

        public override void OnCameraCollision(CollisionComponentEvent collisionComponentEvent)
        {
            if (LayerMask.LayerToName(collisionComponentEvent.ownerGameObject.layer) == "Water")
            {
                ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                FogSettingManager.Setup(_underwaterFogSettingSO);
                FogSettingManager.SetActive(collisionComponentEvent.entered);

                if (CameraMotion.GetComponent<AudioLowPassFilter>() != null)
                    CameraMotion.GetComponent<AudioLowPassFilter>().enabled = collisionComponentEvent.entered;
            }
        }

        private void InputUpdate()
        {
            if (_StartEcs == null)
                return;


            var input = Inatesi.Inputs.Input.GetVector("Move");
            var wishJump = Inatesi.Inputs.Input.Pressed("Jump");
            var wishUse = Inatesi.Inputs.Input.Pressed("Use");
            var wishAttack = Inatesi.Inputs.Input.Pressed("Attack");

            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
            characterComponent.characterMotion.Move(input);


            var zoomInput = Inatesi.Inputs.Input.GetVector("Scroll");
            if (Mathf.Abs(zoomInput.y) > 0)
            {
                playerComponent.cameraMotion.ZoomAmount = zoomInput.y;
            }

            if (wishJump == true && characterComponent.characterMotion.OnGrounded == true)
            {
                characterComponent.characterMotion.AudioSource.PlayOneShot(characterComponent.CharacterSO.AudioCharacter.OnJumpClip);
                characterComponent.characterMotion.AddForce(Vector3.up * characterComponent.characterMotion.MoveConfig.JumpForce);
                characterComponent.characterMotion.AnimatorMonitor.SetAbilityID((int)AnimationAbility.Jump);
            }

            if (Inatesi.Inputs.Input.Down("Jump") && characterComponent.characterMotion.Underwater)
            {
                characterComponent.characterMotion.InputVector3 = Vector3.up * 1f;
                //characterComponent.characterMotion.AddForce(Vector3.up * .2f);

            }
            else
            {
                characterComponent.characterMotion.InputVector3 = Vector3.up * 0f;
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


                    var newEntity = _StartEcs.EcsWorld.NewEntity();
                    ref var useItemEvent = ref _StartEcs.EcsWorld.GetPool<UseItemEvent>().Add(newEntity);
                    useItemEvent.target = collisionEvent ? collisionEvent.gameObject : null;
                }
            }

            if (Inatesi.Inputs.Input.Pressed("1")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(0);
            if (Inatesi.Inputs.Input.Pressed("2")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(1);
            if (Inatesi.Inputs.Input.Pressed("3")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(2);
            if (Inatesi.Inputs.Input.Pressed("4")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(3);
            if (Inatesi.Inputs.Input.Pressed("5")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(4);
            if (Inatesi.Inputs.Input.Pressed("6")) characterComponent.InventoryInteraction2.SetActiveInventoryItem(5);

            if (characterComponent.InventoryInteraction2.CurrentWeaponBase)
                characterComponent.InventoryInteraction2.CurrentWeaponBase.FPC = playerComponent.fpc;



            var wishView = Inatesi.Inputs.Input.Pressed("View");
            if (wishView)
            {
                SetFPSCamera(!FPS);
                characterComponent.characterMotion.FirstPersonSettings.HideObjects(!FPS);
            }
        }
    }
}

