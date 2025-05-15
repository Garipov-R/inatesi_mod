using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace InatesiCharacter.Testing.Decals
{
    public class Decal : MonoBehaviour
    {
        [SerializeField] private DecalProjector _DecalProjector;


        public void Setup(Texture texture)
        {
            if (_DecalProjector == null) return;

            //_DecalProjector.size = new Vector3(texture.width, _DecalProjector.size.y, texture.height);
            _DecalProjector.material.SetTexture("Base_Map", texture);
        }
    }
}