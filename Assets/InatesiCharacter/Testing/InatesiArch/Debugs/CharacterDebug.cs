using InatesiCharacter.SuperCharacter;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.InatesiArch.Debugs
{
    public class CharacterDebug : MonoBehaviour
    {
        [SerializeField] private UIDocument _UIDocument;

        public static CharacterDebug Instance { get; private set; }

        private Label debuglabel;


        private void Awake()
        {
            Instance = this;

            if (_UIDocument == null)
                return;

            debuglabel = _UIDocument.rootVisualElement.Q < Label > ("debug-text");
        }

        public void SetText(string text)
        {
            debuglabel.text = text;
        }
    }
}