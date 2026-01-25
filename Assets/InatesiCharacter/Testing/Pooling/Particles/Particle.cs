using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace InatesiCharacter.Testing.Pooling.Particles
{
    public class Particle : MonoBehaviour
    {
        [SerializeField] private float _s_Timer = .5f;
        private IObjectPool<Particle> _pool;

        public IObjectPool<Particle> Pool { get => _pool; set => _pool = value; }
        public float S_Timer { get => _s_Timer; set => _s_Timer = value; }

        // Use this for initialization
        void Start()
        {

        }


        private float _time = .5f;
        void Update()
        {
            if (enabled == false) return;    
            if (_pool == null) return;

            if (_time < 0)
            {
                _pool.Release(this);

                _time = _s_Timer;
            }
            else
            {
                _time -= Time.deltaTime;
            }
        }
    }
}