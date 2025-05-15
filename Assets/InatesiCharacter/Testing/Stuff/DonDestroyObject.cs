using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Stuff
{
    public class DonDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}