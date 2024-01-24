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
        float duration = 1.0f; 

        canvasGroup.alpha = 0.0f;

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        canvasGroup.alpha = 1.0f;
    }

    IEnumerator FlashObject()
    {

        redFlash.SetActive(true);

        yield return new WaitForSeconds(0.05f);

        redFlash.SetActive(false);

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
    }
}
