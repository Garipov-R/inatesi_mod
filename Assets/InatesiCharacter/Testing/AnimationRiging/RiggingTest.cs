using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace InatesiCharacter.Testing.AnimationRiging
{
    public class RiggingTest : MonoBehaviour
    {
        [Header("targets")]
        [SerializeField] private Transform _Target;

        [Header("RIGs")]
        [SerializeField] private Rig _Rig;
        [SerializeField] private Rig _RightHandRig;
        [SerializeField] private Rig _RightHandCrouchRig;

        [Header("transforms")]
        [SerializeField] private GameObject _SpineRig;
        [SerializeField] private GameObject _RHandRig;
        [SerializeField] private GameObject _LHandRig;
        [SerializeField] private GameObject _Head;

        public Rig Rig { get => _Rig; set => _Rig = value; }
        public GameObject SpineRig { get => _SpineRig; set => _SpineRig = value; }
        public GameObject RHandRig { get => _RHandRig; set => _RHandRig = value; }
        public GameObject LHandRig { get => _LHandRig; set => _LHandRig = value; }
        public GameObject Head { get => _Head; set => _Head = value; }
        public Transform Target { get => _Target; set => _Target = value; }
        public Rig RightHandRig { get => _RightHandRig; set => _RightHandRig = value; }
        public Rig RightHandCrouchRig { get => _RightHandCrouchRig; set => _RightHandCrouchRig = value; }
    }
}