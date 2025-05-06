using System;
using System.Threading.Tasks;

namespace GAOS.EventSystem
{
    /// <summary>
    /// Interface for the event system service that handles event registration, subscription, and triggering
    /// </summary>
    public interface IEventSystem : IDataInterface
    {
        #region Registration
        void RegisterEvent(string eventName);
        void RegisterEvent<T>(string eventName) where T : IDataInterface;
        void RegisterEvent<T, R>(string eventName) where T : IDataInterface where R : IDataInterface;
        
        /// <summary>
        /// Checks if an event with the specified name is already registered
        /// </summary>
        /// <param name="eventName">Event name to check</param>
        /// <returns>True if the event is registered, false otherwise</returns>
        bool IsEventRegistered(string eventName);
        #endregion

        #region Subscription
        void Subscribe(string eventName, Action handler, int priority = 0);
        void Subscribe<T>(string eventName, Action<T> handler, int priority = 0) where T : IDataInterface;
        
        /// <summary>
        /// Subscribe to an event that returns a value asynchronously
        /// </summary>
        void Subscribe<R>(string eventName, Func<Task<R>> handler, int priority = 0) where R : IDataInterface;
        
        /// <summary>
        /// Subscribe to an event with parameters that returns a value asynchronously
        /// </summary>
        void Subscribe<T, R>(string eventName, Func<T, Task<R>> handler, int priority = 0) where T : IDataInterface where R : IDataInterface;
        
        void Unsubscribe(string eventName, Action handler);

        
        void Unsubscribe<T>(string eventName, Action<T> handler);

        
        void Unsubscribe<R>(string eventName, Func<Task<R>> handler);

        
        void Unsubscribe<T, R>(string eventName, Func<T, Task<R>> handler);
        #endregion

        #region Triggering
        void TriggerEvent(string eventName);
        void TriggerEvent<T>(string eventName, T parameters) where T : IDataInterface;
        Task<EventTrigger<R>> TriggerEventAsync<R>(string eventName) where R : IDataInterface;
        Task<EventTrigger<R>> TriggerEventAsync<T, R>(string eventName, T parameters, Func<R, int, int, Task> onListenerCompleted = null) 
            where T : IDataInterface 
            where R : IDataInterface;
        #endregion
    }
} 