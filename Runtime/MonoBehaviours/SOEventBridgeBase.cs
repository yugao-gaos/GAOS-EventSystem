using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GAOS.EventSystem.ScriptableObjects;
using GAOS.ServiceLocator;

namespace GAOS.EventSystem.MonoBehaviours
{
    /// <summary>
    /// Base class for ScriptableObject event to UnityEvent bridges.
    /// Provides common functionality for subscribing, unsubscribing, and event handling.
    /// </summary>
    /// <typeparam name="TSOEvent">The ScriptableObject event type</typeparam>
    /// <typeparam name="TParam">The parameter type for the event</typeparam>
    /// <typeparam name="TResult">The result type for the event</typeparam>
    /// <typeparam name="TUnityEvent">The Unity event type</typeparam>
    public abstract class SOEventBridgeBase<TSOEvent, TParam, TResult, TUnityEvent> : MonoBehaviour
        where TSOEvent : SOEventBase<TParam, TResult>
        where TParam : IDataInterface
        where TResult : IDataInterface
        where TUnityEvent : UnityEvent<TParam, Action<TResult>>
    {
        [SerializeField, Tooltip("The event to subscribe to")]
        protected TSOEvent soEvent;

        [SerializeField, Tooltip("UnityEvent to trigger when the SO event is raised")]
        protected TUnityEvent onEventRaised;

        protected int priority;


        /// <summary>
        /// Gets or sets the ScriptableObject event to listen to
        /// </summary>
        public TSOEvent SOEvent => soEvent;

        public int Priority => priority;

        protected virtual void OnEnable()
        {
            if (soEvent != null)
            {
                SubscribeToEvent();
            }
        }

        protected virtual void OnDisable()
        {
            if (soEvent != null )
            {
                UnsubscribeToEvent();
            }
        }

        /// <summary>
        /// Subscribe to the event with the appropriate delegate
        /// </summary>
        protected abstract void SubscribeToEvent();

           /// <summary>
        /// Unsubscribe to the event with the appropriate delegate
        /// </summary>
        protected abstract void UnsubscribeToEvent();

        /// <summary>
        /// Helper method to execute a callback on the main thread
        /// </summary>
        protected void ExecuteOnMainThread(Action action)
        {
            ServiceCoroutineRunner.RunOnMainThread(action);
        }

        /// <summary>
        /// Creates a Task that will be completed when the callback is called
        /// Useful for bridging between Task-based async and callback-based patterns
        /// </summary>
        protected Task<TResult> CreakTaskForReturnValue(TParam parameter)
        {
            var taskResult = new TaskCompletionSource<TResult>();
            
            ExecuteOnMainThread(() =>
            {
                onEventRaised?.Invoke(parameter, (result) => taskResult.SetResult(result));
            });
            
            return taskResult.Task;
        }
    }
} 