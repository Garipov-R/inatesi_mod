using InatesiCharacter.Testing.Character.Bots;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Components
{
    public struct BotComponent
    {
        public bool enable;
        public IBot Bot;
    }
}