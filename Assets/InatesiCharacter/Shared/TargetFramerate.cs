using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter
{
    public class TargetFramerate : MonoBehaviour
    {
        [SerializeField] [Range(-1f, 200)] private int _Frame = 60;
        [SerializeField] private UIDocument _UIDocument;


        private void OnValidate()
        {
            Application.targetFrameRate = _Frame;
        }   

        private void Start()
        {
#if UNITY_EDITOR
            //_UIDocument.rootVisualElement.SetEnabled(true);
#endif
            Application.targetFrameRate = _Frame;
        }

        private void Update()
        {
            if (_UIDocument != null)
            {
                _UIDocument.rootVisualElement.Q<Label>("fps-counter-text").text = $"{Mathf.RoundToInt(1f / Time.unscaledDeltaTime)}";
            }
        }
    }
}