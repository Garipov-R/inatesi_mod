using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared.Components
{
    public struct DamageComponent
    {
        public float damage;
        public GameObject owner;
        public GameObject target;
        public Vector3 velocity;
        public Vector3 position;
        public object weaponType;
    }
}
