using InatesiCharacter.Camera;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter
{
    public interface ILookSource
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        float LookDirectionDistance { get; }
        float Pitch { get; }
        Vector3 LookPosition();
        Vector3 LookDirection(bool characterLookDirection = false);
        Vector3 LookDirection(Vector3 lookPosition, bool characterLookDirection, int layerMask, bool includeRecoil, bool includeMovementSpread);
        CameraMotion CameraMotion { get; set; }
    }
}