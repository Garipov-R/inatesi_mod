using InatesiCharacter.Testing.Decals;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.SurfaceManagers
{
    public class SurfaceGameObject : MonoBehaviour
    {
        [SerializeField] private MeshDecalData _onHitMeshDecalData;
        [SerializeField] private VisualEffect _onHitVisualEffect;
    }
}