using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InatesiCharacter.Testing.Stuff
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] private string _nameScene = "name";

        private void Start()
        {
            SceneManager.LoadScene(_nameScene);
        }
    }
}