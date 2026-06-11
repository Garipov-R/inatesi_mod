using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.SurfaceManagers
{
    [CreateAssetMenu(fileName = "surface data", menuName = "surface data/new surface data", order = 1)]
    public class SurfaceDataSO : ScriptableObject
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] SurfaceDefinition[] _definedSurfaces;
        [SerializeField] List< RegisteredMaterial> _registeredTextures = new List<RegisteredMaterial>();

        public SurfaceDefinition[] DefinedSurfaces { get => _definedSurfaces; set => _definedSurfaces = value; }
        public List<RegisteredMaterial> RegisteredTextures { get => _registeredTextures; set => _registeredTextures = value; }

        public string[] GetAllSurfaceNames()
        {
            string[] names = new string[_definedSurfaces.Length];

            for (int i = 0; i < names.Length; i++) names[i] = _definedSurfaces[i].name;

            return names;
        }
    }


    [System.Serializable]
    public struct SurfaceDefinition
    {
        public string name;
        public AudioClip[] audioClips;
        public AudioClip[] hitAudioClips;
        public VisualEffect[] visualEffects;
        public Material[] decalMaterials;

    }

    [System.Serializable]
    public struct RegisteredMaterial
    {
        public string textureName;  
        public Texture texture;
        public Texture[] textures;
        public int surfaceIndex;
    }
}