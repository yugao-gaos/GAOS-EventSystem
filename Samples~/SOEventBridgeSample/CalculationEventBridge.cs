using UnityEngine;
using System;
using System.Threading.Tasks;
using GAOS.EventSystem.MonoBehaviours;
using GAOS.EventSystem.ScriptableObjects;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Bridge for the calculation event
    /// </summary>
    [AddComponentMenu("GAOS/Samples/Calculation Event Bridge")]
    public class CalculationEventBridge : SORequestResponseEventBridge<CalculationEvent, ICalculationRequest, ICalculationResult>
    {
        
    }
} 