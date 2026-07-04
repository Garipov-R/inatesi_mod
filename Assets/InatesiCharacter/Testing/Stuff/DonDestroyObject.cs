using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Stuff
{
    public class DonDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            var r = FindObjectsByType<DonDestroyObject>(FindObjectsSortMode.None);

            if (r != null && r.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}