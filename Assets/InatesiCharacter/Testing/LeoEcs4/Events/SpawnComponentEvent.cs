using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Events
{
    public struct SpawnComponentEvent
    {
        public GameObject gameObject;
        public object data;
        public Vector3 position;
        public Quaternion rotation;
        public int entity;
        public bool isPlayer;
    }
}
