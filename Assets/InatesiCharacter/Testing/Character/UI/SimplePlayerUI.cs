using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Character.UI
{
    public class SimplePlayerUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        private VisualElement _rootVisualElement;
        private Label _healthLabel;
        private Label _ammoLabel;
        private VisualElement _screenEffect;

        public Label HealthLabel { get => _healthLabel; set => _healthLabel = value; }

        private void Awake()
        {
            _rootVisualElement = _document.rootVisualElement;
            _healthLabel = _rootVisualElement.Q<Label>("health");
            _ammoLabel = _rootVisualElement.Q<Label>("ammo");
            _screenEffect = _rootVisualElement.Q<VisualElement>("screen-effect");
        }


        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            //_rootVisualElement.Q<Label>("health").text = ;
        }

        public void ScreenEffect(bool enable)
        {
            //_screenEffect.EnableInClassList("enable", enable);
            _screenEffect.EnableInClassList("disable", false);
            StartCoroutine(PerformEffect());
        }

        private IEnumerator PerformEffect()
        {
            yield return new WaitForSeconds(.2f);
            _screenEffect.EnableInClassList("disable", true);
        }

        public T GetVisualElement<T>(string name) where T : VisualElement
        {
            return _rootVisualElement.Q<T>(name);
        }
    }
}