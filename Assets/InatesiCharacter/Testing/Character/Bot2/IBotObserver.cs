using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Bot2
{
    internal interface IBotObserver
    {
        public void DeathBot(GameObject gameObject);
    }
}
