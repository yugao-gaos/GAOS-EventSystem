using System;
using UnityEngine.Events;
using GAOS.EventSystem.ScriptableObjects;

namespace GAOS.EventSystem.MonoBehaviours
{
    /// <summary>
    /// Bridges simple ScriptableObject-based events to UnityEvents.
    /// Handles events with no parameters or return values.
    /// </summary>
    public class SOVoidEventBridge : SOEventBridgeBase<SOVoidEvent, IDataInterface, IDataInterface, UnityEvent<IDataInterface, Action<IDataInterface>>>
    {
      
        /// <summary>
        /// Subscribe to the event with a simple Action
        /// </summary>
        protected override void SubscribeToEvent()
        {
            soEvent.Subscribe(OnEventRaised);
        }   

        /// <summary>
        /// Unsubscribe to the event with a simple Action
        /// </summary>
        protected override void UnsubscribeToEvent()
        {
            soEvent.Unsubscribe(OnEventRaised);
        }
        /// <summary>
        /// Method called when the event is raised
        /// </summary>
        private void OnEventRaised()
        {
            ExecuteOnMainThread(() => onEventRaised?.Invoke(null, null));
        }

    }
} 