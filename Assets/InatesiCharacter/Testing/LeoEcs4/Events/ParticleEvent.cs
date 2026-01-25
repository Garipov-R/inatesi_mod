using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.LeoEcs4.Events
{
    public struct ParticleEvent
    {
        public VisualEffectAsset visualEffectAsset;
        public VisualEffect visualEffect;
        public float speedParticle; // 30 - мега мощный. 15 - райфл. 7 - пистол\дробаш. 1 - нож
        public float sizeParticle;
        public RaycastHit hit;
        public Vector3 position;
        public Vector3 normal;
        public Ray ray;
    }
}
