using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.InatesiArch.WeaponsTest
{
    public interface IWeapon
    {
        public abstract SuperCharacter.CharacterMotionBase CharacterMotion { get; set; }

        public virtual void UpdateTick() { }
        public virtual void Attack() { }
        public virtual void Drop() { }
        public abstract void Init();
        public virtual void Disable() { }
        public virtual void Enable() { }
    }
}
