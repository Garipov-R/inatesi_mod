using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.SuperCharacter
{
    public enum AnimationAbility : byte
    {
        Idle = 0,
        Jump = 1,
        Crouch = 4,
        Die = 3,
        Damage = 5,
        Attack = 6,
    }

    public enum SlotData
    {
        AttackMontirovka = 1
    }
}
