using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3.Shared.Components
{
    public struct SpawnGameObjectEvent
    {
        public GameObject Target;
        public Vector3 Position;
        public Quaternion Quaternion;
        public object data;
    }
}
