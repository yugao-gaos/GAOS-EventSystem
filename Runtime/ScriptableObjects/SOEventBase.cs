using System;
using System.Threading.Tasks;
using UnityEngine;
using GAOS.ServiceLocator;
using GAOS.Logger;

namespace GAOS.EventSystem.ScriptableObjects
{
    /// <summary>
    /// Base ScriptableObject wrapper for the GAOS Event System.
    /// </summary>
    /// <typeparam name="T">The parameter type, must implement IDataInterface</typeparam>
    /// <typeparam name="R">The return type, must implement IDataInterface</typeparam>
    public class SOEventBase<T, R> : ScriptableObject where T : IDataInterface where R : IDataInterface
    {
        [SerializeField, Tooltip("Unique event identifier")]
        protected string eventName;

        [SerializeField, Tooltip("Description for this event")]
        protected string description;

        protected IEventSystem _eventSystem;

        /// <summary>
        /// Gets the event name 
        /// </summary>
        public string EventName => eventName;

        /// <summary>
        /// Gets the description for this event
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Lazily initialized event system
        /// </summary>
        protected IEventSystem EventSystem 
        { 
            get
            {
                if (_eventSystem == null)
                {
                    _eventSystem = GAOS.ServiceLocator.ServiceLocator.GetService<IEventSystem>("EventSystem");
                }
                return _eventSystem;
            }
        }

        /// <summary>
        /// Registers the event in the event system (called automatically on first use)
        /// </summary>
        protected virtual void RegisterEventIfNeeded()
        {
            // Check if the event is already registered
            if (EventSystem.IsEventRegistered(eventName))
                return;
                
            // Register based on the generic parameter types
            bool isDefaultT = typeof(T) == typeof(IDataInterface);
            bool isDefaultR = typeof(R) == typeof(IDataInterface);

            // If both are using the default interface, register as void event
            if (isDefaultT && isDefaultR)
            {
                EventSystem.RegisterEvent(eventName);
            }
            // If only T is non-default, register as parametrized event
            else if (isDefaultR)
            {
                EventSystem.RegisterEvent<T>(eventName);
            }
            // If only R is non-default, register as returning event
            else if (isDefaultT)
            {
                EventSystem.RegisterEvent<R>(eventName);
            }
            // If both are non-default, register with both types
            else
            {
                EventSystem.RegisterEvent<T, R>(eventName);
            }
        }

        /// <summary>
        /// Raises the event with no parameters and no return value
        /// </summary>
        /// <param name="autoRegister">If true, automatically register the event if it's not already registered</param>
        public virtual void Raise(bool autoRegister = true)
        {
            if (autoRegister)
            {
                RegisterEventIfNeeded();
                GLog.Info<EventSystem>($"SOEvent: Raising event '{eventName}' (no parameters, no return value)");
                EventSystem.TriggerEvent(eventName);
            }
            else
            {
                GLog.Info<EventSystem>($"SOEvent: Raising event '{eventName}' (no parameters, no return value)");
                EventSystem.TriggerEvent(eventName, false);
            }
        }

        /// <summary>
        /// Raises the event with parameters but no return value
        /// </summary>
        /// <param name="parameter">The parameter to pass with the event</param>
        /// <param name="autoRegister">If true, automatically register the event if it's not already registered</param>
        public virtual void Raise(T parameter, bool autoRegister = true)
        {
            if (autoRegister)
            {
                RegisterEventIfNeeded();
                GLog.Info<EventSystem>($"SOEvent: Raising event '{eventName}' with parameter");
                EventSystem.TriggerEvent(eventName, parameter);
            }
            else
            {
                GLog.Info<EventSystem>($"SOEvent: Raising event '{eventName}' with parameter");
                EventSystem.TriggerEvent(eventName, parameter, false);
            }
        }

        /// <summary>
        /// Raises the event with no parameters but expects return values
        /// </summary>
        /// <param name="autoRegister">If true, automatically register the event if it's not already registered</param>
        public virtual async Task<EventTrigger<R>> RaiseAsync(bool autoRegister = true)
        {
            if (autoRegister)
            {
                RegisterEventIfNeeded();
                GLog.Info<EventSystem>($"SOEvent: Raising async event '{eventName}' (no parameters)");
                return await EventSystem.TriggerEventAsync<R>(eventName);
            }
            else
            {
                GLog.Info<EventSystem>($"SOEvent: Raising async event '{eventName}' (no parameters)");
                return await EventSystem.TriggerEventAsync<R>(eventName, false);
            }
        }

        /// <summary>
        /// Raises the event with parameters and expects return values
        /// </summary>
        /// <param name="parameter">The parameter to pass with the event</param>
        /// <param name="autoRegister">If true, automatically register the event if it's not already registered</param>
        public virtual async Task<EventTrigger<R>> RaiseAsync(T parameter, bool autoRegister = true)
        {
            if (autoRegister)
            {
                RegisterEventIfNeeded();
                GLog.Info<EventSystem>($"SOEvent: Raising async event '{eventName}' with parameter");
                return await EventSystem.TriggerEventAsync<T, R>(eventName, parameter);
            }
            else
            {
                GLog.Info<EventSystem>($"SOEvent: Raising async event '{eventName}' with parameter");
                return await EventSystem.TriggerEventAsync<T, R>(eventName, parameter, null, false);
            }
        }

        /// <summary>
        /// Subscribes a handler to this event
        /// </summary>
        public virtual void Subscribe(Action handler, int priority = 0)
        {
            RegisterEventIfNeeded();
            EventSystem.Subscribe(eventName, handler, priority);
        }

        /// <summary>
        /// Subscribes a handler with parameters to this event
        /// </summary>
        public virtual void Subscribe(Action<T> handler, int priority = 0)
        {
            RegisterEventIfNeeded();
            EventSystem.Subscribe<T>(eventName, handler, priority);
        }

        /// <summary>
        /// Subscribes a handler with return value to this event
        /// </summary>
        public virtual void Subscribe(Func<Task<R>> handler, int priority = 0)
        {
            RegisterEventIfNeeded();
            EventSystem.Subscribe<R>(eventName, handler, priority);
        }

        /// <summary>
        /// Subscribes a handler with parameters and return value to this event
        /// </summary>
        public virtual void Subscribe(Func<T, Task<R>> handler, int priority = 0)
        {
            RegisterEventIfNeeded();
            EventSystem.Subscribe<T, R>(eventName, handler, priority);
        }

        /// <summary>
        /// Unsubscribes a handler from this event
        /// </summary>
        public virtual void Unsubscribe(Action handler)
        {
            if (EventSystem != null)
            {
                EventSystem.Unsubscribe(eventName, handler);
            }
        }

        /// <summary>
        /// Unsubscribes a handler with parameters to this event
        /// </summary>
         public virtual void Unsubscribe(Action<T> handler)
        {
            if (EventSystem != null)
            {
                EventSystem.Unsubscribe<T>(eventName, handler);
            }
        }

        /// <summary>
        /// Unsubscribes a handler with return value to this event
        /// </summary>
        public virtual void Unsubscribe(Func<Task<R>> handler)
        { 
            if (EventSystem != null)
            {
                EventSystem.Unsubscribe<R>(eventName, handler);
            }
        }

        /// <summary>
        /// Unsubscribes a handler with parameters and return value to this event
        /// </summary>
        public virtual void Unsubscribe(Func<T, Task<R>> handler, int priority = 0)
        {
            
            if (EventSystem != null)
            {
                EventSystem.Unsubscribe<T,R>(eventName, handler);
            }
        }

    }
} 