using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Ragdolls
{
    public class RagdollProperties
    {
        public bool asTrigger;
        public bool isKinematic;
        public bool useGravity = true;
        public bool createTips = true;
        public float rigidDrag;
        public float rigidAngularDrag;
        public CollisionDetectionMode cdMode;
        public bool onlyCollider;

        internal void Draw()
        {
            /*cdMode = (CollisionDetectionMode)EditorGUILayout.EnumPopup("Collision detection:", cdMode);

            rigidDrag = EditorGUILayout.FloatField("Rigid Drag:", rigidDrag);

            rigidAngularDrag = EditorGUILayout.FloatField("Rigid Angular Drag:", rigidAngularDrag);

            asTrigger = EditorGUILayout.Toggle("Trigger colliders:", asTrigger);

            isKinematic = EditorGUILayout.Toggle("Is kinematic:", isKinematic);

            useGravity = EditorGUILayout.Toggle("Use gravity:", useGravity);
            createTips = EditorGUILayout.Toggle("Create tips:", createTips);*/
        }
    }
}
