using UnityEngine;

namespace GAOS.EventSystem.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject wrapper for an event with parameter but no return value
    /// </summary>
    /// <typeparam name="T">The parameter type, must implement IDataInterface</typeparam>
    [CreateAssetMenu(fileName = "NewParameterizedEvent", menuName = "GAOS/Events/Parameterized Event")]
    public class SODataEvent<T> : SOEventBase<T, IDataInterface> where T : IDataInterface
    {
        // This class exists to provide a more specific implementation of SOEventBase
        // with parameters but no return values, for simpler use in the Unity editor.
    }
} 