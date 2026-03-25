using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.UI
{
    [RequireComponent(typeof( GameToolkit.Localization.LocalizedTextBehaviour))]
    public class LabelText : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private string _ElementPath;

        private string _text;

        public string Text 
        {
            set 
            {
                if (_document == null) return;

                try
                {
                    var l = _document.rootVisualElement.Query<Label>().ToList();
                    foreach (var x in l)
                    {
                        if (x.name == _ElementPath)
                        {
                            x.text = value;
                        }
                    }

                    var b = _document.rootVisualElement.Query<Button>().ToList();
                    foreach (var x in b)
                    {
                        if (x.name == _ElementPath)
                        {
                            x.text = value;
                        }
                    }
                }
                catch { }
            }            
        }
    }
}