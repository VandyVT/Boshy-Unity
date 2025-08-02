using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    bool introPlaying = true;
    bool hasPressed = false;

    [SerializeField] GameObject UI_Buttons;
    [SerializeField] GameObject press_Start;
    [SerializeField] GameObject MOTD;
    [SerializeField] ObjectResizer first_Select;
    [SerializeField] Animation boshy_Animation;
    [SerializeField] Animation title_Animation;

    [SerializeField] GameObject[] disableObjects;
    [SerializeField] GameObject[] enableObjects;
    [SerializeField] GameObject enableUiMenu;

    [SerializeField] AudioSource menuMusic;

    [SerializeField] AudioSource stormAudio;
    [SerializeField] AudioClip[] lightningStrike;
    [SerializeField] SpriteRenderer lightningRenderer;

    [SerializeField] GameObject initialMenu;
    [SerializeField] Button gameButton;

    [SerializeField] GameObject saveSelect;
    [SerializeField] Button lastSelectedSave;

    [SerializeField] GameObject deleteSave;
    [SerializeField] Button dontDeleteButton;

    [SerializeField] GameObject difficultySelect;
    [SerializeField] Button hardOnButton;

    [SerializeField] Button[] fileSelections;
    [SerializeField] Button[] difficultySelections;

    [SerializeField] GameObject loadingText;

    [Header("Inputs")]
    [SerializeField] InputActionReference submitAction;
    [SerializeField] InputActionReference escapeAction;
    [SerializeField] InputActionReference deleteAction;

    private void Start()
    {
        LightningStrike();
        StartCoroutine(PlayLightningStrikesRandomly());
        StartCoroutine(MOTD_Timer());
    }

    private void OnEnable()
    {
        submitAction.action.Enable();
        escapeAction.action.Enable();
        deleteAction.action.Enable();
    }

    private void OnDisable()
    {
        submitAction.action.Disable();
        escapeAction.action.Disable();
        deleteAction.action.Disable();
    }

    void Update()
    {
        if (submitAction.action.triggered && introPlaying)
        {
            SkippedIntro();
        }

        if (submitAction.action.triggered && !hasPressed)
        {
            EnableMenu();
            hasPressed = true;
        }

        if (escapeAction.action.triggered)
        {
            if(saveSelect.activeInHierarchy && !difficultySelect.activeInHierarchy)
            {
                initialMenu.SetActive(true); 
                saveSelect.SetActive(false);
                gameButton.Select();
            }

            else if (difficultySelect.activeInHierarchy)
            {
                saveSelect.SetActive(true);
                difficultySelect.SetActive(false);
                lastSelectedSave.Select();
                SetFileInteractivity(true);
            }
        }

        if (deleteAction.action.triggered)
        {
            if (saveSelect.activeSelf)
            {
                deleteSave.SetActive(true);
                dontDeleteButton.Select();
                saveSelect.SetActive(false);
            }
        }

        // Check if the animation has finished playing
        if (!boshy_Animation.isPlaying && introPlaying)
        {
            introPlaying = false;
            title_Animation.Play();
            press_Start.SetActive(true);
            boshy_Animation.Stop();
            Debug.Log("Animation Finished Playing");
        }
    }

    void EnableMenu()
    {
        enableUiMenu.SetActive(true);
        press_Start.SetActive(false);
        first_Select.Resize(true);
        MOTD.SetActive(true);
        StopCoroutine(MOTD_Timer());
    }

    void SkippedIntro()
    {
        introPlaying = false;
        boshy_Animation.Stop();
        title_Animation.Play();
        LightningStrike();

        menuMusic.time = 13.8f;

        foreach (GameObject obj in disableObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in enableObjects)
        {
            obj.SetActive(true);
        }

        Debug.Log("Menu Animation Skipped");
    }

    void LightningStrike()
    {
        if (lightningStrike.Length > 0)
        {
            int randomIndex = Random.Range(0, lightningStrike.Length);
            stormAudio.clip = lightningStrike[randomIndex];
            stormAudio.Play();

            Color startColor = lightningRenderer.color;
            lightningRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0.5f);

            StartCoroutine(FadeOutLightning(lightningRenderer, 1.25f));
        }
        else
        {
            Debug.LogWarning("No lightning strike clips assigned.");
        }
    }

    IEnumerator PlayLightningStrikesRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            LightningStrike();
        }
    }

    IEnumerator MOTD_Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(11);
            MOTD.SetActive(true);
        }
    }

    IEnumerator FadeOutLightning(SpriteRenderer renderer, float duration)
    {
        float currentTime = 0f;
        Color startColor = renderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            renderer.color = Color.Lerp(startColor, targetColor, currentTime / duration);
            yield return null;
        }

        // Ensure the sprite is completely faded out
        renderer.color = targetColor;
    }

    public void SetFileInteractivity(bool setTo)
    {
        foreach (var button in fileSelections)
        {
            if (button != null)
            {
                button.interactable = setTo;
            }
        }
    }

    public void SetDifficultyInteractivity(bool setTo)
    {
        foreach (var button in difficultySelections)
        {
            if (button != null)
            {
                button.interactable = setTo;
            }
        }
    }

    public void DeleteSaveFile()
    {
        GameManager.Instance.DeleteSaveFile();
    }

    public void SetAsLastSelect(Button self)
    {
        lastSelectedSave = self;
    }

    public void GetSavefile(int setSaveNumber)
    {
        GameManager.Instance.saveNumber = setSaveNumber;
    }

    public void SetDifficulty(int setDifficulty)
    {
        GameManager.Instance.difficultyNumber = setDifficulty;
    }

    public void AttemptLoad()
    {
        Debug.Log("Attempting Load");

        string filePath = (Application.persistentDataPath + "/playerData" + GameManager.Instance.saveNumber + ".json");
        if (File.Exists(filePath))
        {
            loadingText.SetActive(true);

            GameManager.Instance.loadPositionOnStart = true;
            GameManager.Instance.LoadPlayerPosition();
        }

        else
        {
            Debug.Log("File doesn't exist, opening up difficulty selection menu.");
            difficultySelect.SetActive(true);
            hardOnButton.Select();
        }
    }
}
