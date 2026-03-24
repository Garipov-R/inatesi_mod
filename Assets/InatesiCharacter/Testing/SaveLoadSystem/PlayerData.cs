using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.SaveLoadSystem
{
    [Serializable]
    public class PlayerData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
        public string SceneName;
        public string[] Weapons;
        public float Health;

        [Serializable]
        public struct Vector3
        {
           public  float x, y, z;
        }
    }
}
