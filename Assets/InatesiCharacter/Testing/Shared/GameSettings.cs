using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace InatesiCharacter.Testing.Shared
{
    public static class GameSettings
    {
        private static bool _inGame;
        private static bool _pause;
        private static Action<bool> _PauseGameAction;
        private static Action<GameSettingsValue> _OnGameValuesChangesAction;
        private static GameSettingsValue _gameSettingsValue = new();
        

        public static bool IsPause { get => _pause; private set => _pause = value; }
        public static Action<bool> PauseGameAction { get => _PauseGameAction; set => _PauseGameAction = value; }
        public static bool InGame { get => _inGame; set => _inGame = value; }
        public static GameSettingsValue GameSettingsValue 
        { 
            get
            {
                _OnGameValuesChangesAction?.Invoke(_gameSettingsValue);
                return _gameSettingsValue;
            } 
            set => _gameSettingsValue = value; 
        }
        public static Action<GameSettingsValue> OnGameValuesChangesAction { get => _OnGameValuesChangesAction; set => _OnGameValuesChangesAction = value; }

        public static void SetPause(bool state)
        {
            UnityEngine.Cursor.lockState = state == true ? CursorLockMode.Confined : CursorLockMode.Locked;
            UnityEngine.Cursor.visible = state;

            _pause = state;
            _PauseGameAction?.Invoke(state);
        }
    }

    public class GameSettingsValue
    {
        private float _mouseSens = 1.92f;
        private float _fov = 80;

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
    }
}
