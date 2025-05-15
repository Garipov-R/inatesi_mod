using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Data
{
    [CreateAssetMenu(fileName = "start weapon character", menuName = "characters/start weapon", order = 1)]
    public class StartWeaponSO : ScriptableObject
    {
        [SerializeField] private List<ItemScriptableObject> _Weapons;

        public List<ItemScriptableObject> Weapons { get => _Weapons; set => _Weapons = value; }
    }
}