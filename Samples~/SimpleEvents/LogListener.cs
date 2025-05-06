using UnityEngine;
using TMPro;
using System.Collections;
using GAOS.EventSystem.Samples;

namespace GAOS.EventSystem.Samples
{
    /// <summary>
    /// Listens to Debug.Log messages to update UI based on events
    /// </summary>
    public class LogListener : MonoBehaviour
    {
        public TextMeshProUGUI feedbackText;
        private Coroutine feedbackCoroutine;
        
        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }
        
        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }
        
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Check for notification message
            if (logString.Contains("Received simple notification"))
            {
                ShowNotificationFeedback();
            }
            
            // Check for calculation result
            if (logString.Contains("Received calculation result:"))
            {
                try
                {
                    // Extract the result value
                    string resultText = logString.Substring(logString.IndexOf(":") + 1).Trim();
                    SimpleEventsUIController uiController = FindObjectOfType<SimpleEventsUIController>();
                    if (uiController != null)
                    {
                        uiController.SetResult($"Result: {resultText}", true);
                    }
                }
                catch
                {
                    // Ignore parsing errors
                }
            }
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
            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(true);
                yield return new WaitForSeconds(2f);
                feedbackText.gameObject.SetActive(false);
            }
        }
    }
} 