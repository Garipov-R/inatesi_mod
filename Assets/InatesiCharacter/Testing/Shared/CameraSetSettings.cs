using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public class CameraSetSettings : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _Camera;

        void Start()
        {
            GameSettings.OnGameValuesChangesAction += (v) => 
            {
                if (_Camera != null)
                    _Camera.fieldOfView = v.Fov;
            };

            if (_Camera != null)
                _Camera.fieldOfView = GameSettings.GameSettingsValue.Fov;
        }
    }
}