using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Bot2;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.UI;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs5
{
    [Serializable]
    public class SharedData
    {
        [SerializeField] private CharacterSO _CharacterSO;
        [SerializeField] private CharacterMotionBase _StartPlayerCharacterMotionBase;
        [SerializeField] private ParticleSettingsSO _ParticleSettingsSO;
        [SerializeField] private List<BotsEvents> _botsEvents=new();

        public CharacterSO CharacterSO { get => _CharacterSO; set => _CharacterSO = value; }
        public CharacterMotionBase StartPlayerCharacterMotionBase { get => _StartPlayerCharacterMotionBase; set => _StartPlayerCharacterMotionBase = value; }
        public ParticleSettingsSO ParticleSettingsSO { get => _ParticleSettingsSO; set => _ParticleSettingsSO = value; }
        public List<BotsEvents> BotsEvents { get => _botsEvents; set => _botsEvents = value; }
    }
}
