using UnityEngine;

namespace GAOS.EventSystem.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject wrapper for a simple event with no parameters or return value
    /// </summary>
    [CreateAssetMenu(fileName = "NewEvent", menuName = "GAOS/Events/Simple Event")]
    public class SOVoidEvent : SOEventBase<IDataInterface, IDataInterface>
    {
        // This class exists to provide a concrete implementation of SOEventBase
        // with no parameters or return values, for simpler use in the Unity editor.
    }
} 