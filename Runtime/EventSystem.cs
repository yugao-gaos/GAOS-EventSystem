using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GAOS.ServiceLocator;
using GAOS.Logger;

namespace GAOS.EventSystem
{
    /// <summary>
    /// Main event system service that handles event registration, subscription, and triggering
    /// </summary>
    [Service(typeof(IEventSystem), "EventSystem", ServiceLifetime.Singleton, ServiceContext.RuntimeAndEditor)]
    public class EventSystem : IEventSystem, ILogSystem
    {
        private readonly ConcurrentDictionary<string, EventRegistration> _events = new();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Delegate, int>> _priorities = new();

        public string LogPrefix => "[EventSystem]";
        public string LogPrefixColor => "#00BFFF";

        public EventSystem()
        {
            GLog.RegisterSystem(this);
            GLog.Info<EventSystem>("Event System initialized");
        }

        #region Registration
        public void RegisterEvent(string eventName)
        {
            _events.TryAdd(eventName, new EventRegistration());
            GLog.Debug<EventSystem>($"Event registered: {eventName}");
        }

        public void RegisterEvent<T>(string eventName) where T : IDataInterface
        {
            _events.TryAdd(eventName, new EventRegistration(typeof(T)));
            GLog.Debug<EventSystem>($"Event registered with data type {typeof(T).Name}: {eventName}");
        }

        public void RegisterEvent<T, R>(string eventName) 
            where T : IDataInterface 
            where R : IDataInterface
        {
            _events.TryAdd(eventName, new EventRegistration(typeof(T), typeof(R)));
            GLog.Debug<EventSystem>($"Event registered with request-response types {typeof(T).Name}->{typeof(R).Name}: {eventName}");
        }

        /// <summary>
        /// Checks if an event with the specified name is already registered
        /// </summary>
        public bool IsEventRegistered(string eventName)
        {
            return _events.ContainsKey(eventName);
        }
        #endregion

        #region Subscription
        public void Subscribe(string eventName, Action handler, int priority = 0)
        {
            ValidateAndAddListener(eventName, handler, priority);
        }

        public void Subscribe<T>(string eventName, Action<T> handler, int priority = 0) 
            where T : IDataInterface
        {
            ValidateAndAddListener(eventName, handler, priority);
        }

        public void Subscribe<R>(string eventName, Func<Task<R>> handler, int priority = 0) 
            where R : IDataInterface
        {
            ValidateAndAddListener(eventName, handler, priority);
        }

        public void Subscribe<T, R>(string eventName, Func<T, Task<R>> handler, int priority = 0) 
            where T : IDataInterface 
            where R : IDataInterface
        {
            ValidateAndAddListener(eventName, handler, priority);
        }

        void IEventSystem.Unsubscribe(string eventName, Action handler)
        {
            UnsubscribeDelegate(eventName, handler);
        }

        void IEventSystem.Unsubscribe<T>(string eventName, Action<T> handler)
        {
            UnsubscribeDelegate(eventName, handler);
        }

        void IEventSystem.Unsubscribe<R>(string eventName, Func<Task<R>> handler)
        {
            UnsubscribeDelegate(eventName, handler);
        }

        void IEventSystem.Unsubscribe<T, R>(string eventName, Func<T, Task<R>> handler)
        {
            UnsubscribeDelegate(eventName, handler);
        }

        public void Unsubscribe(string eventName, Delegate handler)
        {
            UnsubscribeDelegate(eventName, handler);
        }

        private void UnsubscribeDelegate(string eventName, Delegate handler)
        {
            if (_events.TryGetValue(eventName, out var registration))
            {
                registration.RemoveListener(handler);
                if (_priorities.TryGetValue(eventName, out var priorityMap))
                {
                    priorityMap.TryRemove(handler, out _);
                }
                GLog.Debug<EventSystem>($"Removed listener from event: {eventName}");
            }
            else
            {
                GLog.Warning<EventSystem>($"Attempted to unsubscribe from non-existent event: {eventName}");
            }
        }
        #endregion

        #region Triggering
        public void TriggerEvent(string eventName)
        {
            if (!_events.TryGetValue(eventName, out var registration))
            {
                GLog.Warning<EventSystem>($"Attempted to trigger unregistered event: {eventName}");
                return;
            }

            var listeners = GetSortedListeners(eventName, registration);
            GLog.Debug<EventSystem>($"Triggering event {eventName} with {listeners.Count} listeners");

            foreach (var listener in listeners)
            {
                try
                {
                    var action = (Action)listener;
                    action();
                }
                catch (Exception ex)
                {
                    GLog.Error<EventSystem>($"Error executing listener for event {eventName}: {ex.Message}");
                    throw;
                }
            }
        }

        public void TriggerEvent<T>(string eventName, T parameters) where T : IDataInterface
        {
            if (!_events.TryGetValue(eventName, out var registration))
                return;

            var listeners = GetSortedListeners(eventName, registration);
            GLog.Debug<EventSystem>($"Triggering event {eventName} with {listeners.Count} listeners");
            foreach (var listener in listeners)
            {
                try
                {
                    var action = (Action<T>)listener;
                    action(parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle error
                    throw;
                }
            }
        }

        public async Task<EventTrigger<R>> TriggerEventAsync<R>(string eventName) 
            where R : IDataInterface
        {
            if (!_events.TryGetValue(eventName, out var registration))
                return new EventTrigger<R>(0);

            var listeners = GetSortedListeners(eventName, registration);
            GLog.Debug<EventSystem>($"Triggering async event {eventName} with {listeners.Count} listeners");
            var trigger = new EventTrigger<R>(listeners.Count);

            foreach (var listener in listeners)
            {
                if (trigger.CancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var func = (Func<Task<R>>)listener;
                    try
                    {
                        // The handler is now directly awaitable
                        var result = await func().ConfigureAwait(false);
                        trigger.AddResult(result);
                    }
                    catch (OperationCanceledException)
                    {
                        // Task was cancelled, stop processing
                        break;
                    }
                }
                catch (Exception ex)
                {
                    trigger.SetError(ex);
                    throw;
                }
            }

            return trigger;
        }

        public async Task<EventTrigger<R>> TriggerEventAsync<T, R>(
            string eventName, 
            T parameters,
            Func<R, int, int, Task> onListenerCompleted = null) 
            where T : IDataInterface 
            where R : IDataInterface
        {
            if (!_events.TryGetValue(eventName, out var registration))
                return new EventTrigger<R>(0);

            var listeners = GetSortedListeners(eventName, registration);
            GLog.Debug<EventSystem>($"Triggering async event {eventName} with {listeners.Count} listeners");
            var trigger = new EventTrigger<R>(listeners.Count);

            for (int i = 0; i < listeners.Count; i++)
            {
                if (trigger.CancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var func = (Func<T, Task<R>>)listeners[i];
                    try
                    {
                        // The handler is now directly awaitable
                        var result = await func(parameters).ConfigureAwait(false);
                        trigger.AddResult(result);

                        if (onListenerCompleted != null)
                        {
                            await onListenerCompleted(result, trigger.CompletedCount, trigger.TotalListeners);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Task was cancelled, stop processing
                        break;
                    }
                }
                catch (Exception ex)
                {
                    trigger.SetError(ex);
                    throw;
                }
            }

            return trigger;
        }
        #endregion

        #region Helper Methods
        private void ValidateAndAddListener(string eventName, Delegate handler, int priority)
        {
            if (!_events.TryGetValue(eventName, out var registration))
            {
                GLog.Warning<EventSystem>($"Attempted to subscribe to unregistered event: {eventName}");
                throw new ArgumentException($"Event {eventName} is not registered");
            }

            try
            {
                registration.AddListener(handler);
                _priorities.GetOrAdd(eventName, _ => new ConcurrentDictionary<Delegate, int>())[handler] = priority;
                GLog.Debug<EventSystem>($"Added listener to event {eventName} with priority {priority}");
            }
            catch (Exception ex)
            {
                GLog.Error<EventSystem>($"Failed to add listener to event {eventName}: {ex.Message}");
                throw;
            }
        }

        private List<Delegate> GetSortedListeners(string eventName, EventRegistration registration)
        {
            var listeners = registration.GetListeners();
            if (_priorities.TryGetValue(eventName, out var priorityMap))
            {
                return listeners
                    .OrderByDescending(l => priorityMap.TryGetValue(l, out var p) ? p : 0)
                    .ToList();
            }
            return listeners;
        }

        /// <summary>
        /// Clears all registered events and their listeners.
        /// This is primarily used for testing purposes.
        /// </summary>
        public void ClearAllEvents()
        {
            _events.Clear();
            _priorities.Clear();
            GLog.Info<EventSystem>("All events cleared");
        }
        #endregion
    }
} 