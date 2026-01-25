using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class SetupGameSettings : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var sharedData = systems.GetShared<SharedData>();

            if (sharedData == null) return;

            if (sharedData.GameSettingsSO == null) return;

            var gameSettingsValue = sharedData.GameSettingsSO.GameSettingsValue;

            if (gameSettingsValue == null) return;

            var newGameSettingsValue = new GameSettingsValue
            {
                MouseSens = gameSettingsValue.MouseSens,
                Fov = gameSettingsValue.Fov
            };
            UnityEngine.Debug.Log(gameSettingsValue.Fov);
            GameSettings.GameSettingsValue = newGameSettingsValue;
            GameSettings.OnGameValuesChangesAction?.Invoke(GameSettings.GameSettingsValue);
        }
    }
}
