using System.Collections;
using UnityEngine;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] public GameObject gameOver;
    [SerializeField] GameObject redFlash;

    private bool isFlashing = false;

    public static PlayerUiManager instance;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator FadeInGameOver()
    {
        CanvasGroup canvasGroup = gameOver.GetComponent<CanvasGroup>();
        float duration = 1.0f; // Adjust this value based on how quickly you want the alpha to increase

        // Make sure alpha starts at 0
        canvasGroup.alpha = 0.0f;

        // Gradually increase alpha over the specified duration
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure alpha reaches 1.0f exactly
        canvasGroup.alpha = 1.0f;
    }

    IEnumerator FlashObject()
    {
        // Turn the object on
        redFlash.SetActive(true);

        // Wait for 0.05 seconds
        yield return new WaitForSeconds(0.05f);

        // Turn the object off
        redFlash.SetActive(false);

        // Wait for another 0.05 seconds
        yield return new WaitForSeconds(0.05f);
    }

    public void WarningFlash()
    {
        StartCoroutine(FlashObject());
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        StartCoroutine(FadeInGameOver());
        // No need to set alpha here since the coroutine handles it
    }
}
