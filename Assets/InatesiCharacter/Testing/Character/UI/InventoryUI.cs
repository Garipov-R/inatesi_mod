using GameToolkit.Localization;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Character.UI
{
    public class InventoryUI
    {
        private InventoryContainer _InventoryContainer;


        public InventoryUI(InventoryContainer inventoryContainer)
        {
            _InventoryContainer = inventoryContainer;

            Start();
        }

        private void Start()
        {
            UpdateUI();


            _InventoryContainer.OnAdded += (item, active) =>
            {
                UpdateUI();
            };

            _InventoryContainer.OnRemoved += (item) =>
            {
                UpdateUI();
            };

            _InventoryContainer.OnSelected += (item) =>
            {
                UpdateUI();
            };

            GameToolkit.Localization.Localization.Instance.LocaleChanged += Instance_LocaleChanged;
        }

        private void Instance_LocaleChanged(object sender, LocaleChangedEventArgs e)
        {
            UpdateUI();
        }

        public void SetActivePanel(bool active)
        {
            if (RootUI.Instance == null)
            {
                return;
            }

            var panel = RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content");
            panel.style.display = panel.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void UpdateUI(InventoryItem item = null)
        {
            if (RootUI.Instance == null) 
            {
                return; 
            }

            RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content").Clear();

            string emptyText, freeText;

            int id = 0;
            foreach (var inventoryItem in _InventoryContainer.InventoryItems)
            {
                var element = RootUI.Instance.InventoryItem.Instantiate();
                RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content").Add(element);

                element.Q<Label>("name").text = inventoryItem == null ? "" : inventoryItem.Name;
                element.Q<Label>("index").text = inventoryItem == null ? id.ToString() : inventoryItem.SlotIndex.ToString();

                //element.EnableInClassList("default", false);

                if (id == _InventoryContainer.ActiveSlotIndex)
                {
                    //element.EnableInClassList("default:active", true);
                    //element.EnableInClassList("default", false);
                    //element.EnableInClassList("active", true);
                    element.style.backgroundColor = new Color(0, 1, 0, .5f);
                    //element.AddToClassList("active");
                    //element.RemoveFromClassList("default");

                    //element.style.backgroundColor = new StyleColor(new Color(0,0,0,.7f));
                }
                else
                {
                    //element.EnableInClassList("default", true);
                    //element.EnableInClassList("active", false);
                    //element.EnableInClassList("default:active", false);
                    //element.style.backgroundColor = new Color(0,0,0,.5f);
                    element.style.backgroundColor = new Color(0,0,0,.5f);
                    //element.RemoveFromClassList("active");
                    //element.AddToClassList("default");

                    //element.style.backgroundColor = new StyleColor(new Color(0, 1, 0, .7f));
                }

                id++;
            }


            return;
            string debugText = "=== Inventory items ===";

            foreach (var i in _InventoryContainer.InventoryItems)
            {
                if (i != null)
                    debugText += $"\n {i.SlotIndex} | name: {i.Name} | type item: {i.TypeItem}";
                else
                    debugText += $"\n null";
            }

            debugText += $"\n active slot index {_InventoryContainer.ActiveSlotIndex}";
            if (_InventoryContainer.ActiveItem != null) debugText += $"\n active slot { _InventoryContainer.ActiveItem.Name}";
            else debugText += $"\n active slot null";

            RootUI.Instance.UiDocument.rootVisualElement.Q("root").Q<Label>("debug").text = debugText ;
        }
    }
}