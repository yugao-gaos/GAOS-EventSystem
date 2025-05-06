using System;
using UnityEngine.Events;
using GAOS.EventSystem.ScriptableObjects;

namespace GAOS.EventSystem.MonoBehaviours
{
    /// <summary>
    /// Bridges parameterized ScriptableObject-based events to UnityEvents.
    /// Handles events with parameters but no return values.
    /// </summary>
    /// <typeparam name="TParam">The parameter type for the event</typeparam>
    /// <typeparam name="TUnityEvent">The Unity event type to trigger</typeparam>
    public class SODataEventBridge<TParam, TUnityEvent> : 
        SOEventBridgeBase<SODataEvent<TParam>, TParam, IDataInterface, TUnityEvent>
        where TParam : IDataInterface
        where TUnityEvent : UnityEvent<TParam, Action<IDataInterface>>
    {
      
        /// <summary>
        /// Subscribe to the event with an Action that takes a parameter
        /// </summary>
        protected override void SubscribeToEvent()
        {
            soEvent.Subscribe(OnParameterizedEventRaised);
        }

        /// <summary>
        /// Unsubscribe to the event with an Action that takes a parameter
        /// </summary>
        protected override void UnsubscribeToEvent()
        {
            soEvent.Unsubscribe(OnParameterizedEventRaised);
        }

        /// <summary>
        /// Method called when the event is raised
        /// </summary>
        /// <param name="parameter">The parameter from the event</param>
        protected void OnParameterizedEventRaised(TParam parameter)
        {
            ExecuteOnMainThread(() => onEventRaised?.Invoke(parameter, null));
        }

       
    }
} 