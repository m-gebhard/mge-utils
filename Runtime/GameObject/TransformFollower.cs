using UnityEngine;

namespace MGeLabs.Utils.GameObjects
{
    /// <summary>
    /// A simple helper component that follows a target <see cref="UnityEngine.Transform"/> in world space.
    /// Attach to any GameObject to make it move (and optionally rotate) towards another transform.
    /// </summary>
    public class TransformFollower : MonoBehaviour
    {
        [Tooltip("The transform to follow.")]
        [SerializeField] protected Transform target;

        [Header("Position")]
        [Tooltip("If enabled, the follower will move to follow the target's position.")]
        [SerializeField] protected bool followPosition = true;
        [Tooltip("Linear follow speed (units per second).")]
        [SerializeField] protected float followSpeed = 10f;
        [Tooltip("World-space offset added to the target's position.")]
        [SerializeField] protected Vector3 positionOffset;

        [Header("Rotation")]
        [Tooltip("If enabled, the follower will rotate to match the target's rotation.")]
        [SerializeField] protected bool followRotation;
        [Tooltip("Rotation speed in degrees per second.")]
        [SerializeField] protected float rotationSpeed = 180f;

        /// <summary>
        /// Called every frame to update the follower's transform.
        /// </summary>
        protected virtual void Update()
        {
            if (!target) return;

            if (followPosition) FollowPosition();
            if (followRotation) FollowRotation();
        }

        /// <summary>
        /// Moves this follower towards the target's position while applying the configured offset and speed.
        /// </summary>
        protected virtual void FollowPosition()
        {
            Vector3 desiredPosition = target.position + target.TransformDirection(positionOffset);
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates this follower towards the target's rotation at the configured angular speed.
        /// </summary>
        protected virtual void FollowRotation()
        {
            Quaternion desiredRotation = target.rotation;
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        #region Setters

        /// <summary>
        /// Set the target transform to follow.
        /// </summary>
        /// <param name="newTarget">The new target <see cref="UnityEngine.Transform"/>. Pass null to clear the target.</param>
        /// <returns>This instance for fluent/chaining calls.</returns>
        public TransformFollower SetTarget(Transform newTarget)
        {
            target = newTarget;
            return this;
        }

        /// <summary>
        /// Enable or disable position following.
        /// </summary>
        /// <param name="canFollow">If true, the follower will move to match the target's position; if false, position following is disabled.</param>
        /// <returns>The current <see cref="TransformFollower"/> instance for fluent/chaining calls.</returns>
        public TransformFollower SetCanFollowPosition(bool canFollow)
        {
            followPosition = canFollow;
            return this;
        }

        /// <summary>
        /// Set the world-space position offset relative to the target.
        /// </summary>
        /// <param name="newOffset">Offset added to the target's position.</param>
        /// <returns>This instance for fluent/chaining calls.</returns>
        public TransformFollower SetPositionOffset(Vector3 newOffset)
        {
            positionOffset = newOffset;
            return this;
        }

        /// <summary>
        /// Set the follow speed (units per second).
        /// </summary>
        /// <param name="newSpeed">Linear follow speed in units/second.</param>
        /// <returns>This instance for fluent/chaining calls.</returns>
        public TransformFollower SetFollowSpeed(float newSpeed)
        {
            followSpeed = newSpeed;
            return this;
        }

        /// <summary>
        /// Set the rotation speed (degrees per second).
        /// </summary>
        /// <param name="newSpeed">Angular speed in degrees/second. The inspector clamps this between 0 and 360.</param>
        /// <returns>This instance for fluent/chaining calls.</returns>
        public TransformFollower SetRotationSpeed(float newSpeed)
        {
            rotationSpeed = newSpeed;
            return this;
        }

        /// <summary>
        /// Enable or disable rotation following.
        /// </summary>
        /// <param name="canFollow">If true, the follower will rotate to match the target.</param>
        /// <returns>This instance for fluent/chaining calls.</returns>
        public TransformFollower SetCanFollowRotation(bool canFollow)
        {
            followRotation = canFollow;
            return this;
        }

        #endregion
    }
}