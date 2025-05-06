using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor;
using GAOS.EventSystem.Samples;

namespace GAOS.EventSystem.Samples
{
    /// <summary>
    /// Controller for SimpleEvents UI
    /// </summary>
    public class SimpleEventsUIController : MonoBehaviour
    {
        [Header("Object References")]
        public ObjectA objectA;
        
        [Header("UI Elements")]
        public Button notificationButton;
        public Button calculationButton;
        public TMP_InputField valueAInput;
        public TMP_InputField valueBInput;
        public TMP_Dropdown operationDropdown;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI notificationFeedback;
        
        private Coroutine feedbackCoroutine;
        
        private void Start()
        {
            // Make sure we have a reference to ObjectA
            if (objectA == null)
            {
                objectA = FindObjectOfType<ObjectA>();
                if (objectA == null)
                {
                    Debug.LogError("No ObjectA found in the scene!");
                }
            }
            
            // Set up button listeners
            if (notificationButton != null)
            {
                // Remove any existing listeners first
                notificationButton.onClick.RemoveAllListeners();
                // Add the click handler for the notification button
                notificationButton.onClick.AddListener(SendNotification);
            }
            else
            {
                Debug.LogError("Notification Button reference not set!");
            }
            
            if (calculationButton != null)
            {
                // Remove any existing listeners first
                calculationButton.onClick.RemoveAllListeners();
                // Add the click handler for the calculation button
                calculationButton.onClick.AddListener(PerformCalculation);
            }
            else
            {
                Debug.LogError("Calculation Button reference not set!");
            }
            
            // Initialize dropdown if it's empty
            if (operationDropdown != null && operationDropdown.options.Count == 0)
            {
                operationDropdown.options.Clear();
                operationDropdown.options.Add(new TMP_Dropdown.OptionData("add"));
                operationDropdown.options.Add(new TMP_Dropdown.OptionData("subtract"));
                operationDropdown.options.Add(new TMP_Dropdown.OptionData("multiply"));
                operationDropdown.options.Add(new TMP_Dropdown.OptionData("divide"));
                operationDropdown.RefreshShownValue();
            }
        }
        
        /// <summary>
        /// Send simple notification event when notification button is clicked
        /// </summary>
        public void SendNotification()
        {
            if (objectA == null)
            {
                Debug.LogError("ObjectA reference not set!");
                return;
            }
            
            Debug.Log("UI: Sending notification via ObjectA");
            objectA.SendSimpleNotification();
        }
        
        /// <summary>
        /// Perform calculation when calculation button is clicked
        /// </summary>
        public void PerformCalculation()
        {
            if (objectA == null)
            {
                SetResult("Error: ObjectA reference not set", false);
                return;
            }
            
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

            string operation = operationDropdown.options[operationDropdown.value].text.ToLower();
            
            Debug.Log($"UI: Requesting calculation: {valueA} {operation} {valueB}");
            
            // Directly set the values on ObjectA
            objectA.valueA = valueA;
            objectA.valueB = valueB;
            objectA.operation = operation;
            
            // Trigger calculation
            objectA.RequestCalculation();
            
            // We can't directly get the result, so the LogListener will have to update the UI
            SetResult("Waiting for result...", true);
        }
        
        public void ShowNotificationFeedback()
        {
            if (feedbackCoroutine != null)
            {
                StopCoroutine(feedbackCoroutine);
            }
            
            feedbackCoroutine = StartCoroutine(ShowFeedbackCoroutine());
        }
        
        private IEnumerator ShowFeedbackCoroutine()
        {
            if (notificationFeedback != null)
            {
                notificationFeedback.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                notificationFeedback.gameObject.SetActive(false);
            }
        }
        
        public void SetResult(string message, bool success)
        {
            if (resultText != null)
            {
                resultText.text = message;
                resultText.color = success ? Color.green : Color.red;
            }
        }
    }
} 