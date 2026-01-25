using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.LeoEcs4.Components
{
    public struct ParticleComponent
    {
        public float lifetime;
        public IObjectPool<VisualEffect> pool;
        public GameObject gameObject;
        public VisualEffect visualEffect;
        public VisualEffectAsset visualEffectAsset;
        public AudioSource audioSource;
    }
}
