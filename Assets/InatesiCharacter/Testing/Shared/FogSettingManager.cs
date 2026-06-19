using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public static class FogSettingManager
    {
        public static void Setup(FogSettingSO fogSettingSO)
        {
            if (fogSettingSO == null)
            {
                RenderSettings.fog = false;
                return;
            }

            RenderSettings.fogColor = fogSettingSO.FogColor;
            RenderSettings.fogMode = fogSettingSO.FogMode;
            RenderSettings.fogEndDistance = fogSettingSO.End;
            RenderSettings.fogStartDistance = fogSettingSO.Start;
            RenderSettings.fogDensity = fogSettingSO.Density;   
        }

        public static void SetActive(bool state)
        {
            RenderSettings.fog = state;
        }
    }
}