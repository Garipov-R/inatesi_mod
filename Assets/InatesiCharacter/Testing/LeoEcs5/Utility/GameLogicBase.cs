using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Utility
{
    public class GameLogicBase : MonoBehaviour, IGameLogic
    {
        public virtual bool PlayerAlive { get; set; }
        public virtual bool PlayerInput { get; set; }
        public bool FPS 
        {
            get
            {
                if (_StartEcs == null)
                {
                    return false;
                }
                else
                {
                    ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                    return playerComponent.fpc;
                }
            }
            private set
            {
                if (_StartEcs != null)
                {
                    ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                    playerComponent.fpc = value;
                }
            }
        }
        public CameraMotion CameraMotion
        {
            get
            {
                if (_StartEcs != null)
                {
                    ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                    return playerComponent.cameraMotion;
                }
                else
                {
                    return null;
                }
            }
        }
        public GameObject Player { get; set; }


        [Zenject.Inject] protected StartEcs _StartEcs;

        public virtual void GameOver()
        {
            
        }

        public virtual void OnPlayerCollision(CollisionComponentEvent collisionEvent)
        {

        }

        public virtual void OnPlayerDead()
        {
        }

        public virtual void OnPlayerSpawn()
        {

        }

        public virtual void OnPlayerUseItem(bool use)
        {

        }

        public virtual void Pause()
        {
        }

        public virtual void RestartGame()
        {
        }

        public virtual void StartGame()
        {
        }

        public virtual void UpdateUI()
        {

        }

        public virtual void SetFPSCamera(bool state)
        {
            if (_StartEcs == null) return;

            FPS = state;

            CameraMotion.ZoomAmount = FPS ? 0 : 7.5f;
            //CameraMotion.ZoomAmount = FPS ? 0 : 2.5f;

            if (!FPS)
            {
                CameraMotion.AnchorOffset = new Vector3(0.3f, CameraMotion.AnchorOffset.y, CameraMotion.AnchorOffset.z);
            }
            else
            {
                CameraMotion.AnchorOffset = new Vector3(0, CameraMotion.AnchorOffset.y, CameraMotion.AnchorOffset.z);
            }
        }

        public virtual void OnPlayerPickupItem(ItemScriptableObject itemScriptableObject)
        {

        }

        public virtual void OnPlayerSelectItem(ItemScriptableObject itemScriptableObject)
        {
        }

        public virtual void OnPlayerDamage(DamageComponent damageComponent)
        {
        }

        public virtual void OnPlayerLanded()
        {
        }

        public virtual void OnPlayerAimEvent()
        {

        }

        public virtual void OnCameraCollision(CollisionComponentEvent collisionComponentEvent)
        {
        }

        public virtual void OnPlayerShoot()
        {
        }

        public virtual void OnCharacterDead(CharacterComponent characterComponent)
        {

        }

        public virtual void OnCharacterDamage(CharacterComponent characterComponent, DamageComponent damageComponent)
        {
        }

        public Ray GetRay()
        {
            if (_StartEcs == null)
                return new();

            ref var playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
            ref var characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());

            var ray = new Ray();
            var startPoint = characterComponent.characterMotion.transform.position + characterComponent.characterMotion.Up * characterComponent.characterMotion.Height;
            var direction = characterComponent.characterMotion.LookSource.LookDirection();


            if (FPS)
            {
                ray.direction = direction;
                ray.origin = characterComponent.characterMotion.transform.position + characterComponent.characterMotion.Up * characterComponent.characterMotion.Height;
            }
            else
            {
                ray.direction = direction;
                ray.origin = startPoint;
            }

            return ray;
        }

        public bool TryGetPlayerComponent(ref PlayerComponent playerComponent)
        {
            try
            {
                playerComponent = ref ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
        }

        public bool TryGetPlayerCharacterComponent(ref CharacterComponent characterComponent)
        {
            try
            {
                characterComponent = ref ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End());
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }
        }

        public virtual void OnSpawnObject(GameObject targetObject, GameObject sourceObject)
        {
        }

        public virtual void OnReleaseObject(GameObject targetObject)
        {
        }




        public virtual void InputUpdate()
        {
            if (_StartEcs == null)
                return;

            if (PlayerAlive == false)
                return;

            /*if (PlayerInput == false)
                return;*/

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
                //playerComponent.cameraMotion.ZoomAmount = zoomInput.y;
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
                 Configs.Config.s_DefaultLayerMask,
                    QueryTriggerInteraction.Collide
                );

                if (cast)
                {
                    Debug.Log($"{hitInfo.transform.name}");
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

        public virtual void CharacterInputUpdate()
        {
            if (_StartEcs == null)
                return;
        }
    }
}
