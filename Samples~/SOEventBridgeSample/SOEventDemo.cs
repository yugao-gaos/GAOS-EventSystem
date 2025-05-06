using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Demo controller for the SO Event approach
    /// </summary>
    [AddComponentMenu("GAOS/Samples/SO Event Demo")]
    public class SOEventDemo : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private SimpleNotificationEvent notificationEvent;
        [SerializeField] private CalculationEvent calculationEvent;

        [Header("UI Elements")]
        [SerializeField] private Button notificationButton;
        [SerializeField] private Button calculationButton;
        [SerializeField] private TMP_InputField valueAInput;
        [SerializeField] private TMP_InputField valueBInput;
        [SerializeField] private TMP_Dropdown operationDropdown;
        [SerializeField] private TextMeshProUGUI resultText;

        // Operations in the dropdown
        private readonly string[] operations = { "add", "subtract", "multiply", "divide" };

        void Start()
        {
            // Set up button listeners
            if (notificationButton != null)
            {
                notificationButton.onClick.AddListener(SendNotification);
            }

            if (calculationButton != null)
            {
                calculationButton.onClick.AddListener(PerformCalculation);
            }

            // Set up operation dropdown
            if (operationDropdown != null)
            {
                operationDropdown.ClearOptions();
                foreach (string op in operations)
                {
                    operationDropdown.options.Add(new TMP_Dropdown.OptionData(op));
                }
                operationDropdown.value = 0;
                operationDropdown.RefreshShownValue();
            }
        }

        void OnDestroy()
        {
            // Clean up event handlers
            if (notificationButton != null)
            {
                notificationButton.onClick.RemoveListener(SendNotification);
            }

            if (calculationButton != null)
            {
                calculationButton.onClick.RemoveListener(PerformCalculation);
            }
        }

        /// <summary>
        /// Send a simple notification event
        /// </summary>
        public void SendNotification()
        {
            if (notificationEvent != null)
            {
                Debug.Log("Sending notification via SO event");
                notificationEvent.Raise();
            }
        }

        /// <summary>
        /// Perform a calculation using the values from UI
        /// </summary>
        public async void PerformCalculation()
        {
            if (calculationEvent == null || valueAInput == null || 
                valueBInput == null || operationDropdown == null)
                return;

            // Parse input values
            if (!int.TryParse(valueAInput.text, out int valueA))
            {
                SetResult("Invalid Value A", false);
                return;
            }

            if (!int.TryParse(valueBInput.text, out int valueB))
            {
                SetResult("Invalid Value B", false);
                return;
            }

            string operation = operations[operationDropdown.value];
            
            Debug.Log($"Requesting calculation via SO event: {valueA} {operation} {valueB}");
            
            // Create request and trigger the event
            var request = new CalculationRequest(valueA, valueB, operation);
            var trigger = await calculationEvent.RaiseAsync(request);
            
            if (trigger.CompletedResults.Count > 0)
            {
                var result = trigger.CompletedResults[0];
                if (result.Success)
                {
                    SetResult($"Result: {result.Result}", true);
                }
                else
                {
                    SetResult($"Error: {result.ErrorMessage}", false);
                }
            }
            else
            {
                SetResult("No result received", false);
            }
        }

        /// <summary>
        /// Displays the calculation result in the UI
        /// </summary>
        private void SetResult(string message, bool success)
        {
            if (resultText != null)
            {
                resultText.text = message;
                resultText.color = success ? Color.green : Color.red;
            }
        }

    
    }
} 