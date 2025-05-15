using InatesiCharacter.SuperCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks;
using InatesiCharacter.Testing.InatesiArch.InventorySystems;

namespace InatesiCharacter.Testing.InatesiArch.Character
{
    public abstract class CharacterBase
    {
        private GameObject _characterGameObject;
        private CharacterMotionBase _characterMotion;
        private CharacterVars _characterVars = new();
        private List<IAbility> _abilities = new List<IAbility>();
        private InventorySystems.InventoryContainer _inventoryContainer;
        private Vector3 _startPosition;

        public GameObject CharacterGameObject { get => _characterGameObject; }
        public CharacterMotionBase CharacterMotion { get => _characterMotion; set => _characterMotion = value; }
        public CharacterVars CharacterVars { get => _characterVars; set => _characterVars = value; }
        public List<IAbility> Abilities { get => _abilities; set => _abilities = value; }
        public InventoryContainer InventoryContainer { get => _inventoryContainer; set => _inventoryContainer = value; }
        public Vector3 StartPosition { get => _startPosition; set => _startPosition = value; }


        public CharacterBase()
        {
            Debug.Log("==== Character Base");

            Spawn();

            _characterVars = new CharacterVars();

            _inventoryContainer = new InventoryContainer();
        }

        public virtual void Respawn() 
        {
            var characterResource = Resources.Load<GameObject>(Configs.Configs.c_CharacterPath);
            var characterGameObject = GameObject.Instantiate(
                characterResource,
                characterResource.transform.position,
                Quaternion.identity
            );

            _characterGameObject = characterGameObject;

            _characterMotion = characterGameObject.TryGetComponent<CharacterMotionBase>(out _) ? 
                characterGameObject.GetComponent<CharacterMotionBase>() : null;

            if (characterGameObject.TryGetComponent<CharacterMotionBase>(out _) == false)
            {
                _characterMotion.RaycastLayer = ~6;
            }

            _abilities.ForEach(x => x.Init(this));
            _abilities.ForEach(x => x.Start());

            _characterVars.RiggingTest = _characterGameObject.GetComponent<AnimationRiging.RiggingTest>();
            _characterVars.CharacterWorldInteractionSystem = _characterGameObject.GetComponent<CharacterWorldInteractionSystem>();
        }

        public virtual void OnKilled() { }

        public virtual void Spawn() { }

        /// <summary>
        /// update unity method
        /// </summary>
        public virtual void Simulate()  // ( IClient cl )
        {
            _abilities.ForEach(x => x.Update());
        }

        public object GetAbility<T>() where T : IAbility
        {
            return _abilities.FirstOrDefault(x => x is T);
        }
    }
}
