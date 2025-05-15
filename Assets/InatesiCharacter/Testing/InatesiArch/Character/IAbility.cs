using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.InatesiArch.Character
{
    public interface IAbility
    {
        public CharacterBase CharacterBase { get; set; }


        void Start();

        void Init(CharacterBase characterBase) 
        { 
            CharacterBase = characterBase;

            Init();
        }

        void Init();

        void Update();
    }
}
