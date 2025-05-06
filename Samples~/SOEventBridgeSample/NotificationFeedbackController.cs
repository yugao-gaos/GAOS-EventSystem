using UnityEngine;
using TMPro;
using System.Collections;

namespace GAOS.EventSystem.Samples.SOBridge
{
    /// <summary>
    /// Simple controller to show notification feedback
    /// </summary>
    public class NotificationFeedbackController : MonoBehaviour
    {
        public TextMeshProUGUI feedbackText;
        private Coroutine feedbackCoroutine;
        
        public void ShowFeedback()
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