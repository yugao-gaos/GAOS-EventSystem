using UnityEngine;
using GAOS.ServiceLocator;
using GAOS.EventSystem;
using System;
using System.Threading.Tasks;

namespace GAOS.EventSystem.Samples
{
    // GameObject B - Event Receiver
    public class ObjectB : MonoBehaviour
    {
        private IEventSystem _eventSystem;
        
        // Event keys (must match ObjectA)
        private const string SIMPLE_NOTIFICATION = "SimpleNotification";
        private const string CALCULATE_REQUEST = "CalculateRequest";

        // Store delegate references
        private Action _onSimpleNotification;
        private Func<ICalculationRequest, Task<ICalculationResult>> _onCalculateRequest;

        void Start()
        {
            // Get event system service
            _eventSystem = GAOS.ServiceLocator.ServiceLocator.GetService<IEventSystem>("EventSystem");
            
            // Create delegates
            _onSimpleNotification = OnSimpleNotification;
            _onCalculateRequest = OnCalculateRequestAsync;
            
            // Subscribe to events
            _eventSystem.Subscribe(SIMPLE_NOTIFICATION, _onSimpleNotification);
            _eventSystem.Subscribe<ICalculationRequest, ICalculationResult>(
                CALCULATE_REQUEST, _onCalculateRequest);
            
            Debug.Log("Object B initialized and subscribed to events");
        }

        void OnDestroy()
        {
            // Clean up event subscriptions
            if (_eventSystem != null)
            {
                _eventSystem.Unsubscribe(SIMPLE_NOTIFICATION, _onSimpleNotification);
                _eventSystem.Unsubscribe(CALCULATE_REQUEST, _onCalculateRequest);
            }
        }

        // Handler for simple notification
        private void OnSimpleNotification()
        {
            Debug.Log("Object B: Received simple notification from Object A");
        }

        // Handler for calculation request with async return value
        private Task<ICalculationResult> OnCalculateRequestAsync(ICalculationRequest request)
        {
            Debug.Log($"Object B: Processing calculation request - {request.ValueA} {request.Operation} {request.ValueB}");
            
            try
            {
                float result = 0;
                
                switch (request.Operation.ToLower())
                {
                    case "add":
                        result = request.ValueA + request.ValueB;
                        break;
                    case "subtract":
                        result = request.ValueA - request.ValueB;
                        break;
                    case "multiply":
                        result = request.ValueA * request.ValueB;
                        break;
                    case "divide":
                        if (request.ValueB == 0)
                            return Task.FromResult<ICalculationResult>(new CalculationResult(0, false, "Division by zero"));
                        result = (float)request.ValueA / request.ValueB;
                        break;
                    default:
                        return Task.FromResult<ICalculationResult>(new CalculationResult(0, false, $"Unknown operation: {request.Operation}"));
                }
                
                Debug.Log($"Object B: Calculation result is {result}");
                return Task.FromResult<ICalculationResult>(new CalculationResult(result));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Object B: Error during calculation: {ex.Message}");
                return Task.FromResult<ICalculationResult>(new CalculationResult(0, false, ex.Message));
            }
        }
    }
} 