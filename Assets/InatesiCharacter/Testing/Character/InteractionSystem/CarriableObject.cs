using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class CarriableObject : MonoBehaviour
    {
        [SerializeField] private string _NameItem = "Weapon Name"; 
        [SerializeField] private ItemScriptableObject _itemScriptableObject;
        [SerializeField] private CarriableObjectData _CarriableObjectData;

        public string NameItem { get => _NameItem; set => _NameItem = value; }
        public ItemScriptableObject ItemScriptableObject { get => _itemScriptableObject; set => _itemScriptableObject = value; }
        public CarriableObjectData CarriableObjectData { get => _CarriableObjectData; set => _CarriableObjectData = value; }
    }
}