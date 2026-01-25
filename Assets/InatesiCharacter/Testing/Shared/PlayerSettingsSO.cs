using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    [CreateAssetMenu(fileName = "player settings", menuName = "GAME/player setting", order = 1)]
    public class PlayerSettingsSO : ScriptableObject
    {
        [SerializeField] private Texture2D _Crosshair;
        [SerializeField] private Color _CrosshairColor = Color.green;
        [SerializeField] private AudioClip _FlashlightClip;

        public Texture2D Crosshair { get => _Crosshair; set => _Crosshair = value; }
        public Color CrosshairColor { get => _CrosshairColor; set => _CrosshairColor = value; }
        public AudioClip FlashlightClip { get => _FlashlightClip; set => _FlashlightClip = value; }
    }
}
