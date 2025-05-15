using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Movements.SourceEngine.TraceUtility
{
    public struct Trace
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public float fraction;
        public bool startSolid;
        public Collider hitCollider;
        public Vector3 hitPoint;
        public Vector3 planeNormal;
        public float distance;
    }
}
