using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.LeoEcs5;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InatesiCharacter.Testing.LeoEcs5.Components;
using UnityEngine.SceneManagement;
using System;

namespace InatesiCharacter.Testing.SaveLoadSystem
{
    public class SavePlayerTest : MonoBehaviour
    {
        [Zenject.Inject] private StartEcs _StartEcs;

        public void Save()
        {
            InatesiCharacter.Testing.SaveLoadSystem.PlayerData playerData = new InatesiCharacter.Testing.SaveLoadSystem.PlayerData();
            var playerPos = ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld).gameObject.transform.position;
            var playerRot = ECSHelper.Get<PlayerComponent>(_StartEcs.EcsWorld).cameraMotion.LookRotationEuler;
            playerData.Position.x = playerPos.x;
            playerData.Position.y = playerPos.y;
            playerData.Position.z = playerPos.z;
            playerData.Rotation.x = playerRot.x;
            playerData.Rotation.y = playerRot.y;
            playerData.Rotation.z = playerRot.z;
            playerData.Name = "rinat";
            playerData.SceneName = SceneManager.GetActiveScene().name;
            playerData.Health = ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End()).health;

            var inventoryItems = ECSHelper.Get<CharacterComponent>(_StartEcs.EcsWorld, _StartEcs.EcsWorld.Filter<CharacterComponent>().Inc<PlayerComponent>().End()).InventoryInteraction2.InventoryContainer.InventoryItems;
            playerData.Weapons = new string[inventoryItems.Count];
            int i = 0;
            foreach (var item in inventoryItems)
            {
                if (item == null) continue;
                if (item.ItemScriptableObject == null) continue;
                playerData.Weapons[i] = item.ItemScriptableObject.Name;
                i++;
            }

            //playerData.Weapons
            SaveLoad.objects.Clear();
            SaveLoad.objects.Add(playerData);
            SaveLoad.Save();
        }
    }
}