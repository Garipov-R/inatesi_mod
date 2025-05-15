using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Decals
{
    public class DecalsManager : MonoBehaviour
    {
        public static DecalsManager instance;

        [SerializeField] private DecalsDataSO _DecalsDataSO;
        [SerializeField] private Decal _DecalInstance;


        private void Awake()
        {
            instance = this;
        }

        public Decal GetDecal()
        {
            if (_DecalsDataSO == null) return null;


            var textureList = _DecalsDataSO.DecalsData[0].Textures;
            if (textureList == null) return null;
            if (textureList.Count == 0) return null;
            var texture = textureList[Random.Range(0, textureList.Count - 1)];
            if(texture == null) return null;
            var decalInstance = GameObject.Instantiate(_DecalInstance);
            decalInstance.Setup(texture);

            return decalInstance;
        }
    }
}