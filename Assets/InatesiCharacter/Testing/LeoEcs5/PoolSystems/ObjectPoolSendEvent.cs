using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.PoolSystems
{
    public struct ObjectPoolSendEvent
    {
        public GameObject objectToSpawn;
        public Vector3 position;
        public Quaternion rotation;
        public PoolType poolType;
        public object data;
        public Transform parent;
    }
}
