using System;
using UnityEngine;
using UnityEngine.Events;

namespace MGeLabs.Utils.GameObjects
{
    /// <summary>
    /// A utility class that destroys or disables the GameObject after a specified lifetime.
    /// Triggers UnityEvent and Action callbacks before destruction.
    /// </summary>
    public class DestroyAfterTime : MonoBehaviour
    {
        [Tooltip("The lifetime in seconds before the GameObject is destroyed.")]
        [SerializeField] protected float lifetime = 15f;
        [Tooltip("If true, the GameObject will be disabled instead of destroyed.")]
        [SerializeField] protected bool disableOnly;

        /// <summary>
        /// UnityEvent invoked before the GameObject is destroyed.
        /// </summary>
        public UnityEvent OnDestroy;

        /// <summary>
        /// Action event invoked before the GameObject is destroyed.
        /// </summary>
        public event Action OnDestroyAction;

        /// <summary>
        /// Called when the GameObject is enabled. Schedules the destruction of the GameObject.
        /// </summary>
        protected virtual void OnEnable()
        {
            Invoke(nameof(DestroySelf), lifetime);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Cancels the scheduled destruction
        /// of the GameObject.
        /// </summary>
        protected virtual void OnDisable()
        {
            CancelInvoke(nameof(DestroySelf));
        }

        /// <summary>
        /// Invokes the destruction callbacks and destroys or disables the GameObject.
        /// </summary>
        public virtual void DestroySelf()
        {
            OnDestroy?.Invoke();
            OnDestroyAction?.Invoke();

            if (disableOnly) gameObject.SetActive(false);
            else Destroy(gameObject);
        }

        /// <summary>
        /// Sets the lifetime used for scheduled destruction and, if this component is enabled,
        /// reschedules the pending destruction with the new lifetime.
        /// </summary>
        /// <param name="newLifetime">New lifetime in seconds.</param>
        public virtual void SetLifetime(float newLifetime)
        {
            lifetime = Math.Max(0f, newLifetime);

            if (enabled)
            {
                CancelInvoke(nameof(DestroySelf));
                Invoke(nameof(DestroySelf), lifetime);
            }
        }
    }
}