using InatesiCharacter.Testing.Character.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct CharacterInitEvent
    {
        public int entity;
        public CharacterSO CharacterSO;
        public GameObject gameObject;

        public int health;
    }
}
