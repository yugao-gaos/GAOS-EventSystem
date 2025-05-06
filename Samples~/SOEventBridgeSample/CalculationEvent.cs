using UnityEngine;
using GAOS.EventSystem.ScriptableObjects;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Full event with parameters and return values
    /// </summary>
    [CreateAssetMenu(fileName = "CalculationEvent", menuName = "GAOS/Samples/Calculation Event")]
    public class CalculationEvent : SOEventBase<ICalculationRequest, ICalculationResult>
    {
        // Inherits functionality from SOEventBase
    }
} 