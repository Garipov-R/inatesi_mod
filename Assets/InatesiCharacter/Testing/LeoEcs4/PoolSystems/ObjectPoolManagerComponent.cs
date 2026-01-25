using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace InatesiCharacter.Testing.LeoEcs4.Components
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
