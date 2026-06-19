using System;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    [Serializable]
    public class GameSettingsValue
    {
        [SerializeField] private float _mouseSens = 1.92f;
        [SerializeField] private float _fov = 80;
        [SerializeField] private float _GlobalAudioVolume = 1f;
        [SerializeField] private float _MusicVolume = 1f;

        public float MouseSens 
        { 
            get => _mouseSens; 
            set
            {
                _mouseSens = value; 
                _mouseSens = Mathf.Clamp(_mouseSens, 0, 100);
            } 
        }

        public float Fov 
        { 
            get => _fov; 
            set => _fov = Mathf.Clamp(value, 40, 100); 
        }
        public float GlobalAudioVolume { get => _GlobalAudioVolume; set => _GlobalAudioVolume = value; }
        public float MusicVolume { get => _MusicVolume; set => _MusicVolume = value; }
    }
}
