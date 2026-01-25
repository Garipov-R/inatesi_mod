using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Components
{
    public struct BulletProjectileComponent
    {
        public GameObject gameObject;
        public Transform transform;
        public float remainLifeTime;
        public Vector3 moveDirection;
        public float moveSpeed;
        public bool destroyed;
        public float lifetimeAfterDestroy;
        public bool isTriggerOnDestroy;
        public bool isRigidbody;
        public Rigidbody rigidbody;
        public Vector3 addForce;
    }
}
