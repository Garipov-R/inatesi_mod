using InatesiCharacter.Testing.AnimationRiging;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.InatesiArch.Character
{
    public struct CharacterVars
    {
        private bool crouched;
        private bool jumped;
        private bool flying;

        public CharacterVars(CharacterVars cv)
        {
            crouched = cv.Crouched;
            jumped = cv.Jumped;
            flying = cv.flying;
            _riggingTest = null;
            _characterWorldInteractionSystem = null;
        }

        public bool Crouched { readonly get => crouched; set => crouched = value; }
        public bool Jumped { readonly get => jumped; set => jumped = value; }
        public bool Flying { get => flying; set => flying = value; }


        public RiggingTest RiggingTest { get => _riggingTest; set => _riggingTest = value; }
        public CharacterWorldInteractionSystem CharacterWorldInteractionSystem { get => _characterWorldInteractionSystem; set => _characterWorldInteractionSystem = value; }

        private AnimationRiging.RiggingTest _riggingTest;
        private InventorySystems.CharacterWorldInteractionSystem _characterWorldInteractionSystem;
    }
}
