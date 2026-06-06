using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.SuperCharacter
{
    public enum TransitionAnimationState
    {
        Idle = 0,
        Jump = 1,
        Crouch = 4
    }

    public enum WeaponTypeAnimation
    {
        none = 0,
        pistol = 1,
        rifle = 2,
        montirovka = 3,
        granade = 4
    }
}
