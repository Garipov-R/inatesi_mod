using InatesiCharacter.SuperCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public abstract class AbilityBase : IAbility
    {
        protected SuperCharacter.CharacterMotionBase _CharacterMotion;
        protected CharacterVars CharacterVars 
        { 
            get 
            {
                GetCharacterVars(out CharacterVars cv);

                return cv;
            } 
        }

        public CharacterBase CharacterBase { get; set; }
        public CharacterMotionBase CharacterMotion { get => CharacterBase.CharacterMotion;  }


        public void Init()
        {
            _CharacterMotion = CharacterBase.CharacterMotion;
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }

        //public ref CharacterVars GetCharacterVars()
        //{
        //    return ref CharacterVars;
        //}

        public void GetCharacterVars(out CharacterVars cv)
        {
            cv = CharacterBase.CharacterVars;
        }

        public void ApplyCharacterVars(CharacterVars cv)
        {
            CharacterBase.CharacterVars = cv;
        }
    }
}
