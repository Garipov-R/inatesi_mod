using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct BotInitEvent
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }
}
