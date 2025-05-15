using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Stuff
{
    public class platformTest : MonoBehaviour
    {
        [Header("gizmo")]
        [SerializeField] private float _SphereRadius = 1;

        [Header("other")]
        [SerializeField] private GameObject _platform;
        [SerializeField] private Vector3 _GroundRaycastHitPos;
        [SerializeField] private Vector3 _TestVector1;
        [SerializeField] private Vector3 _TestVector2;
        [SerializeField] private Vector3 _LocalPosition;


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            if (_platform == null)
            {
                //Gizmos.DrawWireSphere(transform.position, _SphereRadius / 2);
            }
            else 
            {
                Vector3 ownPos = transform.position;
                Vector3 centerPosPlatform = _platform.transform.position;
                Vector3 finalPos = ownPos + centerPosPlatform;
                //Gizmos.DrawWireSphere(finalPos, _SphereRadius / 2);
            }


            
            Gizmos.color = Color.blue;

            transform.position = Vector3.zero;
            var off = _platform.transform.TransformPoint(_LocalPosition) ;
            Gizmos.DrawSphere(off, 0.25f);

            Gizmos.color = Color.red;
            var of = _platform.transform.TransformDirection(_TestVector2);
            Gizmos.DrawLine(_platform.transform.position, _platform.transform.position + of.normalized * 2f);
            
            
            transform.position = off;
            //transform.position = _platform.transform.TransformPoint(_TestVector1) + localDirection;
        }
    }
}