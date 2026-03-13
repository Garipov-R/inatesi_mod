using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5
{
    [Serializable]
    public class SharedData
    {
        [SerializeField] private CharacterSO _CharacterSO;
        [SerializeField] private CharacterMotionBase _CharacterMotionBase;

        public CharacterSO CharacterSO { get => _CharacterSO; set => _CharacterSO = value; }
        public CharacterMotionBase CharacterMotionBase { get => _CharacterMotionBase; set => _CharacterMotionBase = value; }
    }
}
