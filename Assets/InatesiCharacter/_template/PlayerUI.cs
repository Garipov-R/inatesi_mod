using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter._template
{
	public class PlayerUI : MonoBehaviour
	{
		[SerializeField] private UIDocument _uIDocument;
		private string _healthText = string.Empty;

        public UIDocument UIDocument { get => _uIDocument; set => _uIDocument = value; }


        public void UpdateHealthUI(int health)
		{
			if (_uIDocument == null)
				return;

			if (_healthText == string.Empty)
			{
				_healthText = _uIDocument.rootVisualElement.Q<Label>("health-text").text;
            }

            _uIDocument.rootVisualElement.Q<Label>("health-text").text = string.Format(_healthText, Mathf.Clamp(health, 0, 99999));
			_uIDocument.rootVisualElement.Q<Label>("health-text").style.unityBackgroundImageTintColor = Color.Lerp(Color.red, Color.green, (float)health / 100f);

        }

		public void UpdateAmmoUI(int ammo, int totalAmmo)
		{
            if (_uIDocument == null)
                return;

            if (ammo < 0)
            {
				return;
            }

            _uIDocument.rootVisualElement.Q<Label>("ammo-text").text = $"🧨 {ammo} / {totalAmmo}";

        }
    }
}