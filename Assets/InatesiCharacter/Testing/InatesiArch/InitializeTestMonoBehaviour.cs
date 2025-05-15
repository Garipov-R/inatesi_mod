using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch
{
    public class InitializeTestMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform _SpawnPointPlayer;
        [SerializeField] private bool _FixedUpdateAbility = false;
        [SerializeField] private bool _BOTS = false;

        [SerializeField] private SuperCharacter.CharacterMotionBase[] _CharacterMotions;
        [SerializeField] private Bot.BotTest[] _CharacterMotionBotTests;

        private Game _game1;
        private InatesiCharacter.SuperCharacter.CharacterMotionBase _characterMotionTest;


        private void Awake()
        {
            var game = new Game();
            game.ClientJoined("chaka");

            game.Player.CharacterMotion.SetPositionAndRotation(_SpawnPointPlayer.transform.position, _SpawnPointPlayer.transform.rotation);
            game.Player.StartPosition = _SpawnPointPlayer.transform.position;


            _game1 = game;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_BOTS)
            {
                U();
                U2();

            }

            if (_characterMotionTest == null)
            {
                _characterMotionTest = _game1.Player.CharacterGameObject.GetComponent<InatesiCharacter.SuperCharacter.CharacterMotionBase>();
            }

            if (_FixedUpdateAbility == false) _game1.Player.Simulate();
            _characterMotionTest.UpdateCharacter();

            UpdateCharacters();
        }

        private void FixedUpdate()
        {
            if (_characterMotionTest == null)
            {
                _characterMotionTest = _game1.Player.CharacterGameObject.GetComponent<InatesiCharacter.SuperCharacter.CharacterMotionBase>();
            }

            if (_FixedUpdateAbility == true) _game1.Player.Simulate();
            _characterMotionTest.UpdatePhysicsCharacter();

            UpdatePhysicsCharacters();
        }

        private void UpdateCharacters()
        {
            if (_CharacterMotions == null || _CharacterMotions.Length == 0)
                return;

            foreach (var c in _CharacterMotions)
            {
                c.UpdateCharacter();
            }
        }

        private void UpdatePhysicsCharacters()
        {
            if (_CharacterMotions == null || _CharacterMotions.Length == 0)
                return;

            foreach (var c in _CharacterMotions)
            {
                c.UpdatePhysicsCharacter();
            }
        }

        private void U()
        {
            if (_CharacterMotionBotTests == null || _CharacterMotionBotTests.Length == 0)
                return;

            for (var c = 0; c < _CharacterMotionBotTests.Length; c++)
            {
                _CharacterMotions[c].Move(_CharacterMotionBotTests[c].NavMeshAgent.nextPosition.normalized);
            }
        }

        private void U2()
        {
            if (_CharacterMotionBotTests == null || _CharacterMotionBotTests.Length == 0)
                return;

            for (var c = 0; c < _CharacterMotionBotTests.Length; c++)
            {
                if (_CharacterMotionBotTests[c].AmountHp <= 0)
                {
                    _CharacterMotionBotTests[c].NavMeshAgent.SetDestination(_CharacterMotionBotTests[c].transform.position);
                    continue;
                }
                    

                _CharacterMotionBotTests[c].NavMeshAgent.SetDestination(Game.PlayerInstance.CharacterGameObject.transform.position);
            }
        }
    }
}