using GameToolkit.Localization;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Character.UI
{
    public class InventoryUI
    {
        private InventoryContainer _InventoryContainer;
        private VisualElement[] _slots;
        private VisualElement _InventoryPanel;
        private RootUI _RootUI;

        public RootUI RootUI { get => _RootUI; set => _RootUI = value; }


        public InventoryUI(InventoryContainer inventoryContainer)
        {
            _InventoryContainer = inventoryContainer;

            Init();
            Start();
        }

        private void Init()
        {
            _slots = new VisualElement[_InventoryContainer.Size]; 
            
            if (RootUI.Instance == null)
            {
                return;
            }

            _InventoryPanel = RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content");

            RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content").Clear();

            int id = 0;
            foreach (var inventoryItem in _InventoryContainer.InventoryItems)
            {
                if (RootUI.Instance.InventoryItem == null) { continue; }
                var element = RootUI.Instance.InventoryItem.Instantiate();
                RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content").Add(element);

                string name = string.Empty;

                if (inventoryItem != null)
                {
                    if (inventoryItem.ItemScriptableObject != null)
                    {
                        if (inventoryItem.ItemScriptableObject.LocalizedText != null)
                        {
                            name = inventoryItem.ItemScriptableObject.LocalizedText.Value;
                        }
                        else
                        {
                            name = inventoryItem.Name;
                        }
                    }
                    else
                    {
                        name = inventoryItem.Name;
                    }
                }

                element.Q<Label>("name").text = name;
                element.Q<Label>("index").text = inventoryItem == null ? id.ToString() : inventoryItem.SlotIndex.ToString();
                element.AddToClassList("active");
                element.AddToClassList("inactive");
                element.AddToClassList("default");

                if (id == _InventoryContainer.ActiveSlotIndex)
                {
                    element.EnableInClassList("inactive", false);
                    element.EnableInClassList("active", true);
                }
                else
                {
                    element.EnableInClassList("inactive", true);
                    element.EnableInClassList("active", false);
                }

                _slots[id] = element;
                id++;
            }

            //_InventoryPanel.AddToClassList("enable-content");
            _InventoryPanel.AddToClassList("disable-content");
            //_InventoryPanel.EnableInClassList("enable-content", true);
            _InventoryPanel.EnableInClassList("disable-content", false);
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

        public void TogglePanel(bool active)
        {
            if (RootUI.Instance == null)
            {
                return;
            }

            //var panel = RootUI.Instance.UiDocument.rootVisualElement.Q("inventory-container").Q("content");
            //panel.style.display = panel.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;

            _InventoryPanel.ToggleInClassList("disable-content");


            /*if (_InventoryPanel.style.visibility.value == Visibility.Visible)
            {
                _InventoryPanel.EnableInClassList("enable-content", true);
                _InventoryPanel.EnableInClassList("disable-content", false);
            }
            else
            {
                _InventoryPanel.EnableInClassList("enable-content", false);
                _InventoryPanel.EnableInClassList("disable-content", true);
            }*/
        }

        public void SetActivePanel(bool active)
        {
            if (RootUI.Instance == null)
            {
                return;
            }

            _InventoryPanel.EnableInClassList("disable-content", !active);
        }

        private void UpdateUI()
        {
            if (RootUI.Instance == null)
                return;

            int id = 0;
            foreach (var item in _slots)
            {
                if (item == null) continue;

                var inventoryItem = _InventoryContainer.InventoryItems[id];

                string name = string.Empty;

                if (inventoryItem != null)
                {
                    if (inventoryItem.ItemScriptableObject != null)
                    {
                        if (inventoryItem.ItemScriptableObject.LocalizedText != null)
                        {
                            name = inventoryItem.ItemScriptableObject.LocalizedText.Value;
                        }
                        else
                        {
                            name = inventoryItem.Name;
                        }
                    }
                    else
                    {
                        name = inventoryItem.Name;
                    }
                }

                item.Q<Label>("name").text = name;
                item.Q<Label>("index").text = inventoryItem == null ? id.ToString() : inventoryItem.SlotIndex.ToString();


                if (id == _InventoryContainer.ActiveSlotIndex)
                {
                    item.EnableInClassList("inactive", false);
                    item.EnableInClassList("active", true);
                }
                else
                {
                    item.EnableInClassList("inactive", true);
                    item.EnableInClassList("active", false);
                }

                id++;
            }

            //_InventoryPanel.EnableInClassList("disable-content", true);
        }
    }
}