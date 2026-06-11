using System.Collections;
using UnityEngine;
using EmreBeratKR.IdleCash;
using UnityEngine.UI;

namespace InatesiCharacter.Testing.Stuff
{
    public class MoneyTest : MonoBehaviour
    {
        [SerializeField] private Text _Text;
        private IdleCash idleCashOne;

        private void Start()
        {
            idleCashOne = new IdleCash(0);
            //idleCashOne.value
        }

        private void Update()
        {
            idleCashOne = idleCashOne + new IdleCash(1, idleCashOne.type);
            _Text.text = $"$ {idleCashOne.value.ToString(".0").Replace(",", ".")} {idleCashOne.type}" ;
        }
    }
}