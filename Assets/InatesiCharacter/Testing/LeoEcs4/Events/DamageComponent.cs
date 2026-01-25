using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared.Components
{
    public struct DamageComponent
    {
        public float damage;
        public GameObject owner;
        public GameObject target;
        public Vector3 velocity;
        public Vector3 position;
        public object weaponType;
        public RaycastHit hit;
        public Ray ray;
        public bool isHit;
        public float speedParticle; // 30 - мега мощный. 15 - райфл. 7 - пистол\дробаш. 1 - нож
        public float sizeParticle;
    }
}
