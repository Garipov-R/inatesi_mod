using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Bot3
{

    public class EntityFlagMono : MonoBehaviour
    {
        [SerializeField] private EntityFlag _EntityFlag;

        public EntityFlag EntityFlag { get => _EntityFlag; set => _EntityFlag = value; }
    }
}