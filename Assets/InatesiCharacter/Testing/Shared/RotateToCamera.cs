using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public class RotateToCamera : MonoBehaviour
    {
        [SerializeField] private Transform _mirror;
        private UnityEngine.Camera _camera; 

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            if (_camera != null) 
            {
                Vector3 localPlayer = _mirror.InverseTransformPoint(_camera.transform.position);
                transform.position = _mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, localPlayer.z));

                Vector3 lockAtMirror = _mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
                transform.LookAt(lockAtMirror);
            }
            else
            {
                _camera = UnityEngine.Camera.main;
            }
        }
    }
}