using InatesiCharacter.Testing.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Shared
{
    [CreateAssetMenu(fileName = "Particle settings", menuName = "GAME/particle", order = 1)]
    public class ParticleSettingsSO : ScriptableObject
    {
        [SerializeField] private GameObject _Particle;
        [SerializeField] private AudioSource _AudioSource;
        [SerializeField] private MeshDecal _MeshDecalPrefab;
        [SerializeField] private List<VisualEffect> _VisualEffectList = new();
        [SerializeField] private int _DefaultCapacity = 10;
        [SerializeField] private int _MaxSize = 10000;
        [SerializeField] private bool _CollectionCheck = true;

        public int DefaultCapacity { get => _DefaultCapacity; set => _DefaultCapacity = value; }
        public int MaxSize { get => _MaxSize; set => _MaxSize = value; }
        public bool CollectionCheck { get => _CollectionCheck; set => _CollectionCheck = value; }
        public GameObject Particle { get => _Particle; set => _Particle = value; }
        public List<VisualEffect> VisualEffectList { get => _VisualEffectList; set => _VisualEffectList = value; }
        public AudioSource AudioSource { get => _AudioSource; set => _AudioSource = value; }
        public MeshDecal MeshDecalPrefab { get => _MeshDecalPrefab; set => _MeshDecalPrefab = value; }
    }

    public class ParticleData
    {

    }
}