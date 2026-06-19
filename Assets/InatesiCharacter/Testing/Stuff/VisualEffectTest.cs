using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Stuff
{
    public class VisualEffectTest : MonoBehaviour
    {
        [SerializeField] private List<VisualEffect> effects = new List<VisualEffect>();
        private float time;

        void Update()
        {
            time -= Time.deltaTime;

            if (time < 0)
            {
                foreach (var effect in effects)
                {
                    effect.Play();
                }

                time = 1f;
            }
        }
    }
}