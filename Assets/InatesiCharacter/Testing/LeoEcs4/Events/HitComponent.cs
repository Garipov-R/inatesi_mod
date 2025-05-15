using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3.Shared
{
    public struct HitComponent
    {
        public GameObject first;
        public GameObject other;
        public object[] data;
        public bool isEnter;
    }
}
