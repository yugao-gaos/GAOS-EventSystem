using UnityEngine;
using GAOS.EventSystem;

namespace GAOS.EventSystem.Samples.SOBridge
{
    // Interface for the calculation request data
    public interface ICalculationRequest : IDataInterface
    {
        int ValueA { get; }
        int ValueB { get; }
        string Operation { get; }
    }

    // Implementation of the calculation request interface
    public class CalculationRequest : ICalculationRequest
    {
        public int ValueA { get; set; }
        public int ValueB { get; set; }
        public string Operation { get; set; }

        public CalculationRequest(int a, int b, string operation)
        {
            ValueA = a;
            ValueB = b;
            Operation = operation;
        }
    }

    // Interface for the calculation result data
    public interface ICalculationResult : IDataInterface
    {
        float Result { get; }
        bool Success { get; }
        string ErrorMessage { get; }
    }

    // Implementation of the calculation result interface
    public class CalculationResult : ICalculationResult
    {
        public float Result { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public CalculationResult(float result, bool success = true, string errorMessage = "")
        {
            Result = result;
            Success = success;
            ErrorMessage = errorMessage;
        }
    }
} 