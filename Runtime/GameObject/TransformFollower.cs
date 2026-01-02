using UnityEngine;

namespace MGeLabs.Utils.GameObjects
{
    public class TransformFollower : MonoBehaviour
    {
        [SerializeField] protected Transform target;
        [SerializeField] protected Vector3 positionOffset = Vector3.zero;

        [Header("Smoothing")]
        [SerializeField] protected float smoothTime = 0.15f;
        [SerializeField] protected bool followRotation = false;

        private Vector3 velocity = Vector3.zero;

        protected virtual void LateUpdate()
        {
            if (!target) return;

            // Smooth position
            Vector3 targetPos = target.position + positionOffset;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref velocity,
                smoothTime);

            // Optional smooth rotation
            if (followRotation)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    target.rotation,
                    Time.deltaTime / smoothTime);
            }
        }
    }
}