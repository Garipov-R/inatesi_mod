using System.Collections;
using UnityEngine;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System.Collections.Generic;
using InatesiCharacter.Testing.Character.Bots;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Data
{
    [CreateAssetMenu(fileName = "character", menuName = "characters/new character", order = 1)]
    public class CharacterSO : ScriptableObject
    { 
        [SerializeField] private GameObject _Prefab;
        [SerializeField] private GameObject _Model;
        [SerializeField] private MoveConfig _moveConfig = new ();
        [SerializeField] private AudioCharacter _audioCharacter = new();
        [SerializeField] private StartWeaponSO _startWeaponSO;
        [SerializeField] private CharacterConfig _characterConfig = new CharacterConfig ();

        [Header("settings")]
        [SerializeField][Min(.1f)] private float _ScaleMagnitude = 1;
        [SerializeField] private Material _Material;

        [Header("animation settings")]
        [SerializeField] private bool _RootMotion;
        [SerializeField] private RuntimeAnimatorController _AnimatorController;
        [Header("Effects")]
        [SerializeField] private Material _DissolveMaterial;
        [SerializeField] private VisualEffect _DamageVisualEffect;

        [Space(5)]
        [Header("Bot settings")]
        [SerializeField] private BotConfigs _botConfig = new();


        public MoveConfig MoveConfig { get => _moveConfig; set => _moveConfig = value; }
        public GameObject Prefab { get => _Prefab; set => _Prefab = value; }
        public CharacterConfig CharacterConfig { get => _characterConfig; set => _characterConfig = value; }
        public StartWeaponSO StartWeaponSO { get => _startWeaponSO; set => _startWeaponSO = value; }
        public AudioCharacter AudioCharacter { get => _audioCharacter; set => _audioCharacter = value; }
        public BotConfigs BotConfig { get => _botConfig; set => _botConfig = value; }
        public GameObject Model { get => _Model; set => _Model = value; }
        public bool RootMotion { get => _RootMotion; set => _RootMotion = value; }
        public RuntimeAnimatorController AnimatorController { get => _AnimatorController; set => _AnimatorController = value; }
        public Material Material { get => _Material; set => _Material = value; }
        public float ScaleMagnitude { get => _ScaleMagnitude; set => _ScaleMagnitude = value; }
        public Material DissolveMaterial { get => _DissolveMaterial; set => _DissolveMaterial = value; }
        public VisualEffect DamageVisualEffect { get => _DamageVisualEffect; set => _DamageVisualEffect = value; }
    }

    [System.Serializable]
    public class AudioCharacter
    {
        [SerializeField] private AudioClip[] _HurtClips;
        [SerializeField] private AudioClip _RebornClip;
        [SerializeField] private float _VolumeHeart = 1;

        public AudioClip[] HurtClips { get => _HurtClips; set => _HurtClips = value; }
        public AudioClip RebornClip { get => _RebornClip; set => _RebornClip = value; }
        public float VolumeHurt { get => _VolumeHeart; set => _VolumeHeart = value; }
    }

    

    [System.Serializable]
    public class CharacterConfig
    {
        [SerializeField] private float _startHealth = 100;
        [SerializeField] private float _TimeAfterDeath = 3;

        public float StartHealth { get => _startHealth; set => _startHealth = value; }
        public float TimeAfterDeath { get => _TimeAfterDeath; set => _TimeAfterDeath = value; }
    }

    [System.Serializable]
    public class BotConfigs
    {
        [SerializeField] private GameObject _BotComponent;

        public GameObject BotComponent { get => _BotComponent; set => _BotComponent = value; }
    }
}