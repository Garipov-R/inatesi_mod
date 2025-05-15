using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.SuperCharacter
{
    public interface ICharacter
    {
        public abstract void UpdateCharacterMethod();
        public abstract void FixedUpdateCharacterMethod();
    }
}
