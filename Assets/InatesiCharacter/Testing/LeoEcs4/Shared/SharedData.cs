using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.InteractionSystem;
using InatesiCharacter.Testing.Character.UI;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs
{
    [Serializable]
    public class SharedData
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private CharacterSO _PlayerCharacterSO;
        [SerializeField] private GameSettingsSO _GameSettingsSO;
        [SerializeField] private RootUI _RootUI;
        [SerializeField] private PlayerSettingsSO _PlayerSettingsSO;
        [SerializeField] private ParticleSettingsSO _ParticleSettingsSO;

        public Transform SpawnPoint { get => _spawnPoint; set => _spawnPoint = value; }
        public CharacterSO PlayerCharacterSO { get => _PlayerCharacterSO; set => _PlayerCharacterSO = value; }
        public GameSettingsSO GameSettingsSO { get => _GameSettingsSO; set => _GameSettingsSO = value; }
        public RootUI RootUI { get => _RootUI; set => _RootUI = value; }
        public PlayerSettingsSO PlayerSettingsSO { get => _PlayerSettingsSO; set => _PlayerSettingsSO = value; }
        public ParticleSettingsSO ParticleSettingsSO { get => _ParticleSettingsSO; set => _ParticleSettingsSO = value; }
    }
}
