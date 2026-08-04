using InatesiCharacter.Testing.Decals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.LeoEcs5.Components
{
    public struct BulletProjectileComponent
    {
        public GameObject gameObject;
        public LayerMask ignoreLayer;
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
        public GameObject owner;
        public VisualEffect explosionVfxPrefab;
        public AudioClip explosionClip;
        public MeshDecal explosionMeshDecal;
        public Material explosionDecalMaterial;
    }
}
