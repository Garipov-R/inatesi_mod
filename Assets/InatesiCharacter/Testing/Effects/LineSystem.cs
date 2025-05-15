using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InatesiCharacter.Testing.Effects
{
    public class LineSystem : MonoBehaviour
    {
        [SerializeField] private int _Size = 512;
        [SerializeField] private LineRenderer _ShootLineRenderer;
        [SerializeField] private List<LineRenderer> _LineRenderers ;

            
        private Dictionary<LineRenderer, float> _LineRendererTimeSince = new Dictionary<LineRenderer, float>();


        private static LineSystem _Instance;

        public static LineSystem Instance { get => _Instance; set => _Instance = value; }


        private void Awake()
        {
            _Instance = this;
                                                                           
            for (int i = 0; i < _Size; i++)
            {
                var item = Instantiate(_ShootLineRenderer);
                item.gameObject.SetActive(false);

                item.transform.SetParent(transform, false);

                _LineRenderers.Add(item);
                
            }
        }

        private void FixedUpdate()
        {
            if (_LineRendererTimeSince == null) 
            {
                _LineRendererTimeSince = new Dictionary<LineRenderer, float>();
                return; 
            }

            if (_LineRendererTimeSince.Count == 0) 
            {
                //_LineRendererTimeSince = new Dictionary<LineRenderer, float>();
                return; 
            }

            foreach (var item in _LineRenderers)
            {
                if (_LineRendererTimeSince.TryGetValue(item, out float timeSince) == true)
                {
                    if (timeSince <= 0)
                    {
                        item.gameObject.SetActive(false);
                    }
                    else
                    {
                        item.widthMultiplier = Mathf.Lerp(item.widthMultiplier, 0, Time.deltaTime * 15);
;                        _LineRendererTimeSince[item] = timeSince = timeSince - Time.deltaTime;
                    }
                }
            }
        }

        public void SetLine(Vector3 startPosition, Vector3 endPosition)
        {
            foreach (var item in _LineRenderers)
            {
                if (item.gameObject.activeSelf == true)
                    continue;

                if (_LineRendererTimeSince.TryGetValue(item, out float timeSince) == true)
                {
                    item.widthMultiplier = .1f;
                    _LineRendererTimeSince[item] = timeSince = 1;
                }
                else
                {
                    _LineRendererTimeSince.Add(item, 1);
                }
                

                Vector3[] positions = { startPosition, endPosition };
                item.SetPositions(positions);
                item.gameObject.SetActive(true);

                break;
            }
        }
    }
}