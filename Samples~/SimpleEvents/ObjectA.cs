using UnityEngine;
using GAOS.ServiceLocator;
using GAOS.EventSystem;
using System.Threading.Tasks;

namespace GAOS.EventSystem.Samples
{
    // GameObject A - Event Sender
    public class ObjectA : MonoBehaviour
    {
        private IEventSystem _eventSystem;
        
        // Event keys
        private const string SIMPLE_NOTIFICATION = "SimpleNotification";
        private const string CALCULATE_REQUEST = "CalculateRequest";
        
        // For demonstration in the inspector - making these public for direct access
        public int valueA = 10;
        public int valueB = 5;
        public string operation = "add"; // add, subtract, multiply, divide

        void Start()
        {
            // Get event system service
            _eventSystem = GAOS.ServiceLocator.ServiceLocator.GetService<IEventSystem>("EventSystem");
            
            // Register events
            _eventSystem.RegisterEvent(SIMPLE_NOTIFICATION);
            _eventSystem.RegisterEvent<ICalculationRequest, ICalculationResult>(CALCULATE_REQUEST);
            
            Debug.Log("Object A initialized and events registered");
        }

        // Called from UI or other triggers
        public void SendSimpleNotification()
        {
            Debug.Log("Object A: Sending simple notification");
            _eventSystem.TriggerEvent(SIMPLE_NOTIFICATION);
        }

        // Called from UI or other triggers
        public async void RequestCalculation()
        {
            var request = new CalculationRequest(valueA, valueB, operation);
            Debug.Log($"Object A: Requesting calculation - {request.ValueA} {request.Operation} {request.ValueB}");
            
            var trigger = await _eventSystem.TriggerEventAsync<ICalculationRequest, ICalculationResult>(
                CALCULATE_REQUEST, request);
            
            if (trigger.CompletedResults.Count > 0)
            {
                var result = trigger.CompletedResults[0];
                if (result.Success)
                {
                    Debug.Log($"Object A: Received calculation result: {result.Result}");
                }
                else
                {
                    Debug.LogError($"Object A: Calculation failed: {result.ErrorMessage}");
                }
            }
            else
            {
                Debug.LogWarning("Object A: No results received from calculation request");
            }
        }
    }
} 