using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Movements.SourceEngine.TraceUtility
{
    public class Tracer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="origin"></param>
        /// <param name="end"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static Trace TraceCollider(
            Collider collider, 
            Vector3 origin, 
            Vector3 end, 
            int layerMask, 
            float colliderScale = 1f
        )
        {

            if (collider is BoxCollider)
            {

                // Box collider trace
                return TraceBox(origin, end, collider.bounds.extents, collider.contactOffset, layerMask, colliderScale);

            }
            else if (collider is CapsuleCollider)
            {

                // Capsule collider trace
                var capc = (CapsuleCollider)collider;

                Vector3 point1, point2;
                
                SurfPhysics.GetCapsulePoints(capc, origin, out point1, out point2);

                return TraceCapsule(point1, point2, capc.radius, origin, end, capc.contactOffset, layerMask, colliderScale);

            }
            else if (collider is CharacterController)
            {
                // Capsule collider trace
                var character = (CharacterController)collider;

                Vector3 point1, point2;

                SurfPhysics.GetCharacterControllerPoints(character, origin, out point1, out point2);

                return TraceCapsule(point1, point2, character.radius, origin, end, character.contactOffset, layerMask, colliderScale);
            }

            throw new System.NotImplementedException("Trace missing for collider: " + collider.GetType());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Trace TraceCapsule(
            Vector3 point1, 
            Vector3 point2, 
            float radius, 
            Vector3 start, 
            Vector3 destination, 
            float contactOffset, 
            int layerMask, 
            float colliderScale = 1f
        )
        {

            var result = new Trace()
            {
                startPos = start,
                endPos = destination
            };

            var longSide = Mathf.Sqrt(contactOffset * contactOffset + contactOffset * contactOffset);
            radius *= (1f - contactOffset);
            var direction = (destination - start).normalized;
            var maxDistance = Vector3.Distance(start, destination) + longSide;

            RaycastHit hit;
            if (Physics.CapsuleCast(
                point1: point1 - Vector3.up * colliderScale * 0.5f,
                point2: point2 + Vector3.up * colliderScale * 0.5f,
                radius: radius * colliderScale,
                direction: direction,
                hitInfo: out hit,
                maxDistance: maxDistance,
                layerMask: layerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore))
            {

                result.fraction = hit.distance / maxDistance;
                result.hitCollider = hit.collider;
                result.hitPoint = hit.point;
                result.planeNormal = hit.normal;
                result.distance = hit.distance;

                RaycastHit normalHit;
                Ray normalRay = new Ray(hit.point - direction * 0.001f, direction);
                if (hit.collider.Raycast(normalRay, out normalHit, 0.002f))
                {

                    result.planeNormal = normalHit.normal;

                }

            }
            else
                result.fraction = 1;

            return result;

        }

        public static int CharacterOverlap(
            Vector3 point1,
            Vector3 point2,
            Vector3 position, 
            Quaternion rotation, 
            CapsuleCollider capsuleCollider,
            Collider[] overlappedColliders, 
            LayerMask layers, 
            QueryTriggerInteraction triggerInteraction, 
            float inflate = 0f
        )
        {
            Vector3 bottom = position + (rotation * point1);
            Vector3 top = position + (rotation * point2);
            if (inflate != 0f)
            {
                bottom += (rotation * Vector3.down * inflate);
                top += (rotation * Vector3.up * inflate);
            }

            int nbHits = 0;
            int nbUnfilteredHits = Physics.OverlapCapsuleNonAlloc(
                        bottom,
                        top,
                        capsuleCollider.radius + inflate,
                        overlappedColliders,
                        layers,
                        triggerInteraction);

            // Filter out the character capsule itself
            nbHits = nbUnfilteredHits;
            for (int i = nbUnfilteredHits - 1; i >= 0; i--)
            {
                if (overlappedColliders[i] == capsuleCollider)
                {
                    nbHits--;
                    if (i < nbHits)
                    {
                        overlappedColliders[i] = overlappedColliders[nbHits];
                    }
                }
            }

            return nbHits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Trace TraceBox(
            Vector3 start, 
            Vector3 destination, 
            Vector3 extents, 
            float contactOffset, 
            int layerMask, 
            float colliderScale = 1f
        )
        {

            var result = new Trace()
            {
                startPos = start,
                endPos = destination
            };

            var longSide = Mathf.Sqrt(contactOffset * contactOffset + contactOffset * contactOffset);
            var direction = (destination - start).normalized;
            var maxDistance = Vector3.Distance(start, destination) + longSide;
            extents *= (1f - contactOffset);

            RaycastHit hit;
            if (Physics.BoxCast(center: start,
                halfExtents: extents * colliderScale,
                direction: direction,
                orientation: Quaternion.identity,
                maxDistance: maxDistance,
                hitInfo: out hit,
                layerMask: layerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore))
            {

                result.fraction = hit.distance / maxDistance;
                result.hitCollider = hit.collider;
                result.hitPoint = hit.point;
                result.planeNormal = hit.normal;
                result.distance = hit.distance;

                RaycastHit normalHit;
                Ray normalRay = new Ray(hit.point - direction * 0.001f, direction);
                if (hit.collider.Raycast(normalRay, out normalHit, 0.002f))
                {

                    result.planeNormal = normalHit.normal;

                }

            }
            else
                result.fraction = 1;

            return result;

        }

    }
}
