using UnityEngine;
using GAOS.EventSystem.ScriptableObjects;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Simple event with no parameters or return values
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleNotificationEvent", menuName = "GAOS/Samples/Simple Notification Event")]
    public class SimpleNotificationEvent : SOVoidEvent
    {
        // Inherits functionality from SOEvent
    }
} 