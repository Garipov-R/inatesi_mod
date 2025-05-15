using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter.Tools
{
    public class RigBulderEditor : MonoBehaviour
    {
        [ContextMenu("Build")]
        public void Build()
        {
            TryGetComponent(out UnityEngine.Animations.Rigging.RigBuilder rigBuilder);

            if (rigBuilder != null)
            {
                rigBuilder.Build();
            }
        }
    }
}