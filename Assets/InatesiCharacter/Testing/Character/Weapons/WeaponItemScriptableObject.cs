using InatesiCharacter.Testing.Character.Weapons;
using SwayBobItemSO = InatesiCharacter.Testing.InatesiArch.WeaponsTest.SwayBobItemSO;
using System.Collections;
using UnityEngine;
using InatesiCharacter.Testing.Character.InteractionSystem;


namespace InatesiCharacter.Testing.InatesiArch.InventorySystems
{
    [CreateAssetMenu(fileName = "weapon item name", menuName = "Inventory items/Weapon", order = 1)]
    public class WeaponItemScriptableObject : ItemScriptableObject
    {
        [SerializeField] private GameObject _ViewModel;
        [SerializeField] private SwayBobItemSO _SwayBobItemSO;
        [SerializeField] private WeaponBase _weaponBase;
        [SerializeField] private CarriableObjectData _carriableObjectData = new();

        public WeaponItemScriptableObject()
        {
            _itemType = TypeItem.Weapon;
        }

        public GameObject ViewModel { get => _ViewModel; set => _ViewModel = value; }
        public SwayBobItemSO SwayBobItemSO { get => _SwayBobItemSO; set => _SwayBobItemSO = value; }
        public WeaponBase Weapon { get => _weaponBase; set => _weaponBase = value; }
        public CarriableObjectData CarriableObjectData { get => _carriableObjectData; set => _carriableObjectData = value; }
    }
}
