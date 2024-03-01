using System;
using System.Collections;
using UnityEngine;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField] public GameObject gameOver;
    [SerializeField] GameObject redFlash;
    [SerializeField] GameObject[] teleportUI;
    [SerializeField] PSXEffects _pixelationFX;

    private bool isFlashing = false;

    public static PlayerUiManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance.isWarping)
        {
            teleportUI[1].SetActive(true);
            GameManager.Instance.isWarping = false;
        }
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

    public void Teleporting()
    {
        if (GameManager.Instance.isWarping) 
        {
            teleportUI[0].SetActive(true);
        }
        else
        {
            teleportUI[1].SetActive(true);
        }

        StartCoroutine(Pixelize());
        _pixelationFX.enabled = true;
    }

    IEnumerator Pixelize()
    {
        float timer = 0f;
        float duration = 2f;

        int startResolutionFactor = 0;
        int targetResolutionFactor = 150;

        startResolutionFactor = _pixelationFX.resolutionFactor;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float lerpValue = Mathf.Lerp(startResolutionFactor, targetResolutionFactor, timer / duration);
            _pixelationFX.resolutionFactor = (int)lerpValue; // Cast the float to int
            yield return null; // Wait for the next frame
        }
    }
}
