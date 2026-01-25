using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public static class G
    {
        [SerializeField] private static bool _MuteAudio = false;
        private static Action<bool> _OnPauseGameAction;
        private static bool _IsPause;

        public static bool MuteAudio { get => _MuteAudio; set => _MuteAudio = value; }
        public static Action<bool> OnPauseGameAction { get => _OnPauseGameAction; set => _OnPauseGameAction = value; }
        public static bool IsPause { get => _IsPause; set => _IsPause = value; }

        public static void SetPause(bool state)
        {
            _IsPause = state;
            _OnPauseGameAction?.Invoke(state);
            Time.timeScale = _IsPause ? 0 : 1;
            //Debug.Log(Time.timeScale);
        }

        public static void SetVisibleCursor(bool state)
        {
            UnityEngine.Cursor.lockState = state == true ? CursorLockMode.Confined : CursorLockMode.Locked;
            UnityEngine.Cursor.visible = state;
        }
    }
}
