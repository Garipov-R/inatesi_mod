using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.Character.UI;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3
{
    [Serializable]
    public class SharedData
    {
        [SerializeField] private Transform _spawnPoint;
        private AudioDatas _audioDatas;
        private PlayerInventoryInteraction _playerInventoryInteraction;
        private WeaponsDatas _weaponDatas;
        private PlayerData _playerData = new PlayerData();
        [SerializeField] private CharacterSO _PlayerCharacterSO;

        public Transform SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }
        public AudioDatas AudioDatas { get => _audioDatas; }
        public PlayerInventoryInteraction PlayerInventoryInteraction { get => _playerInventoryInteraction; set => _playerInventoryInteraction = value; }
        public WeaponsDatas WeaponDatas { get => _weaponDatas; set => _weaponDatas = value; }
        public PlayerData PlayerData { get => _playerData; set => _playerData = value; }
        public CharacterSO PlayerCharacterSO { get => _PlayerCharacterSO; set => _PlayerCharacterSO = value; }
    }

    [Serializable]
    public class AudioDatas
    {
        [SerializeField] private AudioClip _ScreamAudioClip;
        [SerializeField] private AudioClip _DieScreamAudioClip;
        [SerializeField] private AudioClip _RebornAudioClip;
        [SerializeField] private List<AudioClip> _HurtCharacterAudioClipList = new List<AudioClip>();

        public AudioClip ScreamAudioClip { get => _ScreamAudioClip; set => _ScreamAudioClip = value; }
        public AudioClip DieScreamAudioClip { get => _DieScreamAudioClip; set => _DieScreamAudioClip = value; }
        public AudioClip RebornAudioClip { get => _RebornAudioClip; set => _RebornAudioClip = value; }
        public List<AudioClip> HurtCharacterAudioClipList { get => _HurtCharacterAudioClipList; set => _HurtCharacterAudioClipList = value; }
    }

    public class PlayerInventoryInteraction
    {
        private InventoryContainer _InventoryContainer;
        private InventoryInteraction _inventoryInteraction;
        private InventoryUI _inventoryUI;

        public PlayerInventoryInteraction(CharacterMotionBase characterMotionBase)
        {
            GameObject.FindObjectOfType<RootUI>().Awakee();
            _InventoryContainer = new InventoryContainer();
            _inventoryInteraction = new InventoryInteraction(_InventoryContainer, characterMotionBase);
            _inventoryUI = new InventoryUI(_InventoryContainer);
        }

        public InventoryContainer InventoryContainer { get => _InventoryContainer; set => _InventoryContainer = value; }
        public InventoryInteraction InventoryInteraction { get => _inventoryInteraction; set => _inventoryInteraction = value; }
        public InventoryUI InventoryUI { get => _inventoryUI; set => _inventoryUI = value; }
    }

    [System.Serializable]
    public class WeaponsDatas
    {
        [SerializeField] private List<ItemScriptableObject> _Weapons;

        public List<ItemScriptableObject> Weapons { get => _Weapons; set => _Weapons = value; }
    }

    public class PlayerData
    {
        private bool _fps;

        public bool Fps { get => _fps; set => _fps = value; }
    }
}
