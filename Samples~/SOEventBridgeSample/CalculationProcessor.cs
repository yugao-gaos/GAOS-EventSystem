using UnityEngine;
using System;
using GAOS.EventSystem.Samples.SOBridge;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Standalone calculator component that performs calculations.
    /// This can be hooked up to the CalculationEventBridge through Unity Events in the editor.
    /// </summary>
    [AddComponentMenu("GAOS/Samples/Calculation Processor")]
    public class CalculationProcessor : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool logResults = true;

        /// <summary>
        /// Process a calculation request and return the result through the provided callback
        /// </summary>
        /// <param name="request">The calculation request containing operation and values</param>
        /// <param name="callback">Callback to return the result</param>
        public void ProcessCalculation(ICalculationRequest request, Action<ICalculationResult> callback)
        {
            if (request == null)
            {
                SendResult(callback, 0, false, "Invalid request: null");
                return;
            }

            if (logResults)
            {
                Debug.Log($"Calculation Processor: Processing {request.ValueA} {request.Operation} {request.ValueB}");
            }

            try
            {
                float result = 0;
                bool success = true;
                string errorMessage = string.Empty;
                
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
                        {
                            result = 0;
                            success = false;
                            errorMessage = "Division by zero";
                        }
                        else
                        {
                            result = (float)request.ValueA / request.ValueB;
                        }
                        break;
                    default:
                        result = 0;
                        success = false;
                        errorMessage = $"Unknown operation: {request.Operation}";
                        break;
                }

                SendResult(callback, result, success, errorMessage);
            }
            catch (Exception ex)
            {
                SendResult(callback, 0, false, ex.Message);
            }
        }

        private void SendResult(Action<ICalculationResult> callback, float result, bool success, string errorMessage = "")
        {
            if (logResults)
            {
                if (success)
                {
                    Debug.Log($"Calculation Processor: Result = {result}");
                }
                else
                {
                    Debug.LogWarning($"Calculation Processor: Error = {errorMessage}");
                }
            }

            var calculationResult = new CalculationResult(result, success, errorMessage);
            callback?.Invoke(calculationResult);
        }
    }
} 