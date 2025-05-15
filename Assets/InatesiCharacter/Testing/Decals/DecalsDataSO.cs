using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Decals
{
    [CreateAssetMenu(fileName = "decal", menuName = "Decals", order = 1)]
    public class DecalsDataSO : ScriptableObject
    {
        [Serializable]
        public class DecalData 
        {
            [SerializeField] private string name;
            [SerializeField] private List<Texture> textures;

            public string Name { get => name; set => name = value; }
            public List<Texture> Textures { get => textures; set => textures = value; }
        }

        [SerializeField] private List<DecalData> _DecalsData;

        public List<DecalData> DecalsData { get => _DecalsData; set => _DecalsData = value; }
    }
}
