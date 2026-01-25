using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    [CreateAssetMenu(fileName = "game settings", menuName = "GAME/create game setting", order = 1)]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeField] private GameSettingsValue _GameSettingsValue;

        public GameSettingsValue GameSettingsValue { get => _GameSettingsValue; set => _GameSettingsValue = value; }
    }
}