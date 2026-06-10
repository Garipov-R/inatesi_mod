using InatesiCharacter.Camera;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.LeoEcs5.Components;
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

            CameraMotion.ZoomAmount = FPS ? 0 : 2.5f;

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
    }
}
