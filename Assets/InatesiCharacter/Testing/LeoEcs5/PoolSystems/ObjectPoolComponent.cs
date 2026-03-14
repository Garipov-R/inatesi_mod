using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.PoolSystems
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
