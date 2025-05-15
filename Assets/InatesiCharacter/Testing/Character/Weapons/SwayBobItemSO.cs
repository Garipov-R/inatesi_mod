using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    [CreateAssetMenu(fileName = "item name", menuName = "Inventory items/Sway Bob", order = 1)]
    public class SwayBobItemSO : ScriptableObject
    {
        [SerializeField] private SwayBob _swayBob = new SwayBob();

        public SwayBob SwayBob { get => _swayBob; }
    }
}