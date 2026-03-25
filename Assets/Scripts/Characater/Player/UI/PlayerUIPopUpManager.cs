using System.Collections;
using UnityEngine;
using TMPro;
namespace CFS
{

    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("YOU DIED Pop Up")] [SerializeField]
        private GameObject youDiedPopUp;

        [SerializeField] private TextMeshProUGUI youDiedPopUpBackgroundText;
        [SerializeField] private TextMeshProUGUI youDiedPopUpText;

        [SerializeField]
        private CanvasGroup youDiedPopUpCanvasGroup; // Allows alpha to be changed for fade in/out effect

        public void SendYouDiedPopUp()
        {
            // Post - Processing Effects

            youDiedPopUp.SetActive(true);
            youDiedPopUpBackgroundText.characterSpacing = 0f;
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 5f, 20f));

            StartCoroutine(FadeCanvasGroup(youDiedPopUpCanvasGroup, 1f, 5f));

            StartCoroutine(FadeCanvasGroup(youDiedPopUpCanvasGroup, 0f, 3f, 5f));
            // Fade Out The Pop Up

        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                text.characterSpacing =
                    Mathf.Lerp(0f, stretchAmount,
                        elapsedTime / duration); // Adjust the target character spacing as needed
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            text.characterSpacing = 100f; // Ensure it reaches the final value

            /*
            if (duration > 0f)
            {
                text.characterSpacing = 0;
                float timer = 0;
                yield return null;

                while (duration < timer)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(0, stretchAmount, duration * Time.deltaTime / 20);

                }
            }
            */
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration, float delay = 0)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                while (delay > 0f)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha; // Ensure it reaches the final value
            /*
            if (duration > 0f)
            {
                canvasGroup.alpha = 0;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(0, targetAlpha, timer / duration);
                }
            }

            canvasGroup.alpha = 1;
            yield return null;
            */
        }
    }
}