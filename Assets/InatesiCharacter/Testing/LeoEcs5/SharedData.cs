using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.Character.UI;
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
        [SerializeField] private UIDocument _PlayerUIDocument;
        [SerializeField] private SimplePlayerUI _SimplePlayerUI;

        public CharacterSO CharacterSO { get => _CharacterSO; set => _CharacterSO = value; }
        public CharacterMotionBase StartPlayerCharacterMotionBase { get => _StartPlayerCharacterMotionBase; set => _StartPlayerCharacterMotionBase = value; }
        public ParticleSettingsSO ParticleSettingsSO { get => _ParticleSettingsSO; set => _ParticleSettingsSO = value; }
        public UIDocument PlayerUIDocument { get => _PlayerUIDocument; set => _PlayerUIDocument = value; }
        public SimplePlayerUI SimplePlayerUI { get => _SimplePlayerUI; set => _SimplePlayerUI = value; }
    }
}
