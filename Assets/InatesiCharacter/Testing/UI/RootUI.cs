using InatesiCharacter.Testing.Stuff;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.UI
{
    public class RootUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _inventoryItem;
        [SerializeField] private UIDocument _PropPanelUIDocument;


        public static RootUI Instance;
        public UIDocument UiDocument { get => _uiDocument; set => _uiDocument = value; }
        public VisualTreeAsset InventoryItem { get => _inventoryItem; set => _inventoryItem = value; }
        public UIDocument PropPanelUIDocument { get => _PropPanelUIDocument; set => _PropPanelUIDocument = value; }


        private void Awake()
        {
            Awakee();
        }
        public void Awakee()
        {
            Instance = this;
        }
    }
}