using InatesiCharacter.Testing.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.PoolSystems
{
    public struct ObjectPoolComponent
    {
        public float lifeTime;
        public GameObject gameObject;
        public PoolType poolType;
        public int index;
        public GameObject prefab;

        public bool isTimer;
    }
}
