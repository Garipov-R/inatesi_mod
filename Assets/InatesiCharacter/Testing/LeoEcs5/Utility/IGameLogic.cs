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
    public interface IGameLogic
    {
        public bool PlayerAlive { get; set; }
        public bool FPS { get; }
        public CameraMotion CameraMotion { get; }
        public GameObject Player {  get; set; }

        public void StartGame();

        public void GameOver();

        public void Pause();

        public void RestartGame();

        public void OnPlayerDead();

        public void OnPlayerSpawn();

        public void OnPlayerCollision(CollisionComponentEvent collisionEvent);

        public void UpdateUI();

        public void OnPlayerUseItem(bool use);

        public void OnPlayerPickupItem(ItemScriptableObject itemScriptableObject);

        public void OnPlayerSelectItem(ItemScriptableObject itemScriptableObject);

        public void OnPlayerDamage(DamageComponent damageComponent);

        public void OnPlayerLanded();

        public void OnPlayerAimEvent();

        public void OnPlayerShoot();

        public void OnCameraCollision(CollisionComponentEvent collisionComponentEvent);

        public void OnCharacterDead(CharacterComponent characterComponent);
        public void OnCharacterDamage(CharacterComponent characterComponent, DamageComponent damageComponent);
    }
}
