using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using GAOS.EventSystem.ScriptableObjects;
using GAOS.ServiceLocator;

namespace GAOS.EventSystem.MonoBehaviours
{
    /// <summary>
    /// UnityEvent for request-response patterns that passes both the request and a callback for the response
    /// </summary>
    /// <typeparam name="TParam">Parameter type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    [Serializable]
    public class RequestResponseUnityEvent<TParam, TResult> : UnityEvent<TParam, Action<TResult>> 
        where TParam : IDataInterface 
        where TResult : IDataInterface
    { }

    /// <summary>
    /// Bridges ScriptableObject-based request-response events to UnityEvents.
    /// Handles events with both parameters and return values.
    /// </summary>
    /// <typeparam name="TSOEvent">The ScriptableObject event type</typeparam>
    /// <typeparam name="TParam">The parameter type for the event</typeparam>
    /// <typeparam name="TResult">The result type for the event</typeparam>
    public class SORequestResponseEventBridge<TSOEvent, TParam, TResult> : 
        SOEventBridgeBase<TSOEvent, TParam, TResult, RequestResponseUnityEvent<TParam, TResult>>
        where TSOEvent : SOEventBase<TParam, TResult>
        where TParam : IDataInterface
        where TResult : IDataInterface
    {

        /// <summary>
        /// Subscribe to the event with an Action that takes a parameter
        /// </summary>
        protected override void SubscribeToEvent()
        {
            soEvent.Subscribe(OnRequestAsync);
        }

        /// <summary>
        /// Unsubscribe to the event with an Action that takes a parameter
        /// </summary>
        protected override void UnsubscribeToEvent()
        {
            soEvent.Unsubscribe(OnRequestAsync);
        }

        /// <summary>
        /// Method called when the event is raised
        /// Uses TaskCompletionSource to bridge between async and callback patterns
        /// </summary>
        protected virtual Task<TResult> OnRequestAsync(TParam parameter)
        {
            return CreakTaskForReturnValue(parameter);
        }
    }
} 