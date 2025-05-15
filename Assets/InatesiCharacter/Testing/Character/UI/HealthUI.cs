using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Character.UI
{
    public class HealthUI
    {
        public void UpdateUI(float hp)
        {
            if (RootUI.Instance == null)
            {
                return;
            }

            RootUI.Instance.UiDocument.rootVisualElement.Q<Label>("health-amount").text = hp > 0 ? hp.ToString("0") : "0";

            var color = Color.Lerp(Color.red, Color.white,  hp / 40);

            RootUI.Instance.UiDocument.rootVisualElement.Q<Label>("health-amount").style.color = color;
            RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("heart-icon").style.unityBackgroundImageTintColor = color;
        }

        public void Reset()
        {
            if (RootUI.Instance == null) return;

            RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("pain-panel").style.backgroundColor = new StyleColor(Color.clear);
        }

        public void ProcessUpdate()
        {
            if (RootUI.Instance == null) return;

            var color = RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("pain-panel").style.backgroundColor.value;
            color = Color.Lerp(color, Color.clear, Time.deltaTime * 1.4f);
            RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("pain-panel").style.backgroundColor = new StyleColor(color);
        }

        public void SetColorPanel(Color color, bool alpha = true)
        {
            if(RootUI.Instance == null) return;

            RootUI.Instance.UiDocument.rootVisualElement.Q<VisualElement>("pain-panel").style.backgroundColor = new StyleColor(new Color(color.r, color.g, color.b, color.a));
        }

        public void Pain(float hp)
        {
            SetColorPanel(new Color(1, 0, 0, .3f));
        }
    }
}