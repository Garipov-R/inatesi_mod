using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace InatesiCharacter.Testing.LeoEcs5.PoolSystems
{
    public struct ObjectPoolManagerComponent
    {
        public GameObject particlesEmpty;
        public GameObject gameObjectEmpty;
        public GameObject soundFXEmpty;
        public GameObject emptyHolder;

        public Dictionary<GameObject, ObjectPool<GameObject>> objectsPool;
        public Dictionary<GameObject, GameObject> clonePrefabMap;


    }
}
