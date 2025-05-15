using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.InatesiArch.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _inventoryItem;


        private void Start()
        {
            var player = Game.PlayerInstance as Player;

            if (player == null)
                return;

            for (int i = 0; i < player.InventoryContainer.InventoryItems.Count; i++)
            {
                var item = player.InventoryContainer.InventoryItems[i];
                var itemUI = _inventoryItem.Instantiate();
                _uiDocument.rootVisualElement.Q("inventory-container").Q("content").Add(itemUI);

                itemUI.Q<Label>("name").text = item == null ?  "empty" : item.Name;
                itemUI.Q<Label>("index").text = item == null ? "free" : item.SlotIndex.ToString();
            }

            player.InventoryContainer.OnAdded += (item, active) => 
            {
                var child = _uiDocument.rootVisualElement.Q("inventory-container").Q("content").Children();
                foreach (var i in child)
                {
                    if (i.Q<Label>("index").text == "free")
                    {
                        i.Q<Label>("name").text = item.Name;
                        i.Q<Label>("index").text = item.SlotIndex.ToString();
                        if (item.ItemScriptableObject.Sprite) 
                            i.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(item.ItemScriptableObject.Sprite);

                        break;
                    }
                }
            };

            player.InventoryContainer.OnRemoved += (item) =>
            {
                var child = _uiDocument.rootVisualElement.Q("inventory-container").Q("content").Children();
                foreach (var element in child)
                {
                    if (element.Q<Label>("name").text == item.Name)
                    {
                        element.Q<Label>("name").text ="empty";
                        element.Q<Label>("index").text =  "free";
                        element.Q<VisualElement>("image").style.backgroundImage = new StyleBackground();

                        break;
                    }
                }
            };

            _uiDocument.rootVisualElement.Q("inventory-container").Q("content").Clear();
        }

        public void Update()
        {
            return;

            var player = Game.PlayerInstance as Player;

            if (player == null)
                return;



            string debugText = "=== Inventory items ===";

            foreach (var item in player.InventoryContainer.InventoryItems)
            {
                if (item != null)
                    debugText += $"\n name: {item.Name} | type item: {item.TypeItem}";
            }

            debugText += $"\n active slot index {player.InventoryContainer.ActiveSlotIndex}";

            Debugs.CharacterDebug.Instance.SetText(debugText);
        }
    }
}