using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    [CreateAssetMenu(fileName = "fog_setting", menuName = "GAME/fog settings", order = 1)]
    public class FogSettingSO : ScriptableObject
    {
        public Color FogColor = Color.purple;
        public FogMode FogMode;
        [Range(0, 1)]public float Density = .5f;
        public float Start;
        public float End;
    }
}