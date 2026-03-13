using InatesiCharacter.Testing.Character.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct BotComponent
    {
        public GameObject gameObject;
        public int health;
        public bool died;

        public BotTest botTest;
    }
}
