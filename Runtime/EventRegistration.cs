using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GAOS.EventSystem
{
    /// <summary>
    /// Encapsulates the registration data for a single event type
    /// </summary>
    internal class EventRegistration
    {
        private readonly List<Delegate> _listeners;
        private readonly object _lock = new object();

        /// <summary>
        /// The type of parameters this event accepts
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// The type of return value this event produces
        /// </summary>
        public Type ReturnType { get; }

        public EventRegistration(Type parameterType = null, Type returnType = null)
        {
            ValidateType(parameterType, "Parameter");
            ValidateType(returnType, "Return");

            ParameterType = parameterType;
            ReturnType = returnType;
            _listeners = new List<Delegate>();
        }

        private void ValidateType(Type type, string typeName)
        {
            if (type == null) return;
            if (type == typeof(void)) return;

            if (!typeof(IDataInterface).IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"{typeName} type must implement IDataInterface",
                    nameof(type));
            }

            if (!type.IsInterface)
            {
                throw new ArgumentException(
                    $"{typeName} type must be an interface",
                    nameof(type));
            }
        }

        /// <summary>
        /// Adds a listener to this event
        /// </summary>
        public void AddListener(Delegate listener)
        {
            ValidateListener(listener);
            
            lock (_lock)
            {
                _listeners.Add(listener);
            }
        }

        /// <summary>
        /// Removes a listener from this event
        /// </summary>
        public bool RemoveListener(Delegate listener)
        {
            lock (_lock)
            {
                return _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Gets a snapshot of current listeners
        /// </summary>
        public List<Delegate> GetListeners()
        {
            lock (_lock)
            {
                return new List<Delegate>(_listeners);
            }
        }

        private void ValidateListener(Delegate listener)
        {
            var method = listener.Method;
            var parameters = method.GetParameters();

            // Validate parameter type
            if (ParameterType != null)
            {
                if (parameters.Length != 1 || !ParameterType.IsAssignableFrom(parameters[0].ParameterType))
                {
                    throw new ArgumentException(
                        $"Listener must accept a single parameter of type {ParameterType.Name}",
                        nameof(listener));
                }
            }
            else if (parameters.Length != 0)
            {
                throw new ArgumentException(
                    "Listener must not have parameters for parameterless event",
                    nameof(listener));
            }

            // Validate return type
            var returnType = method.ReturnType;
            if (ReturnType != null)
            {
                // Handle async methods
                if (typeof(Task).IsAssignableFrom(returnType))
                {
                    if (!returnType.IsGenericType || 
                        !ReturnType.IsAssignableFrom(returnType.GetGenericArguments()[0]))
                    {
                        throw new ArgumentException(
                            $"Async listener must return Task<{ReturnType.Name}>",
                            nameof(listener));
                    }
                }
                else if (!ReturnType.IsAssignableFrom(returnType))
                {
                    throw new ArgumentException(
                        $"Listener must return {ReturnType.Name}",
                        nameof(listener));
                }
            }
            else if (returnType != typeof(void) && returnType != typeof(Task))
            {
                throw new ArgumentException(
                    "Listener must return void for events without return type",
                    nameof(listener));
            }
        }
    }
} 