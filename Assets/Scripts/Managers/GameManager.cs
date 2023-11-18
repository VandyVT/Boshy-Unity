using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string fileName = "playerPosition.json";

    [SerializeField] AudioSource[] _levelMusic;
    [SerializeField] AudioSource _gameOverMusic;
    [SerializeField] float fadeTime = 1.0f; // Time in seconds to fade the pitch to zero

    [SerializeField] bool playMusicOnStart;

    public Text debugText;

    public int difficultyNumber; // 0 = Ez | 1 = Average | 2 = Hard | 3 = Rage
    public int saveNumber;

    [SerializeField] bool loadPosition;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Screen.fullScreen = false;
    }

    void Start()
    {
        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null && !audioSource.isPlaying && playMusicOnStart)
            {
                audioSource.Play();
            }
        }

        LoadPlayerPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerCharacter.instance.Restart();
            PlayerUiManager.instance.gameOver.SetActive(false);

            ResetTheme();
        }
    }

    void ResetTheme()
    {
        StopAllCoroutines();

        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null)
            {
                audioSource.pitch = 1f;
            }
        }

        _gameOverMusic.Stop();
    }

    IEnumerator FadeThemePitch()
    {
        float currentTime = 0;
        float initialPitch = _levelMusic[0].pitch;

        while (currentTime < fadeTime)
        {
            // Calculate the normalized time (0 to 1)
            float normalizedTime = currentTime / fadeTime;

            // Use Mathf.Lerp to gradually change the pitch to zero
            foreach (var audioSource in _levelMusic)
            {
                if (audioSource != null)
                {
                    audioSource.pitch = Mathf.Lerp(initialPitch, 0f, normalizedTime);
                }
            }

            // Increment the time
            currentTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final pitch is exactly zero
        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null)
            {
                audioSource.pitch = 0f;
            }
        }
    }

    public void GameOver()
    {
        StartCoroutine(FadeThemePitch());
        PlayerUiManager.instance.GameOver();
        _gameOverMusic.Play();
    }

    public void SavePlayerPosition()
    {
        debugText.text = "";
        debugText.text += ("Starting save operation to " + Application.persistentDataPath);

        // Create JSON object
        JSONObject playerData = new JSONObject();
        playerData["position"]["x"].AsFloat = PlayerCharacter.instance.transform.position.x;
        playerData["position"]["y"].AsFloat = PlayerCharacter.instance.transform.position.y;

        // Get the file path in StreamingAssets
        string filePath = (Application.persistentDataPath + "/playerData" + saveNumber + ".json");

        // Write JSON data to the file
        File.WriteAllText(filePath, playerData.ToString());

        Debug.Log("Player position saved!");

        //CODE
        debugText.text += ("Data saved to: " + Application.persistentDataPath);

        if (!File.Exists(Application.persistentDataPath + "/playerData.json")) debugText.text += "\nEXCEPT NOT REALLY!!";
    }

    public void LoadPlayerPosition()
    {
        if (!loadPosition) return;
        // Get the file path in StreamingAssets
        string filePath = (Application.persistentDataPath + "/playerData" + saveNumber + ".json");

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read JSON data from the file
            string jsonData = File.ReadAllText(filePath);

            // Parse JSON data
            JSONObject playerData = JSON.Parse(jsonData) as JSONObject;

            // Retrieve player position
            float x = playerData["position"]["x"].AsFloat;
            float y = playerData["position"]["y"].AsFloat;

            // Set player position
            PlayerCharacter.instance.lastSavedPostion = new Vector2(x, y);

            Debug.Log("Player position loaded: " + PlayerCharacter.instance.transform.position);
        }

        else
        {
            Debug.Log("No saved player data found. Using default position.");
        }
    }
}
