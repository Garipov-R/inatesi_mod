using InatesiCharacter.Testing.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.PoolSystems
{
    public struct ObjectPoolSendEvent
    {
        public GameObject objectToSpawn;
        public Vector3 position;
        public Quaternion rotation;
        public PoolType poolType;
    }
}
