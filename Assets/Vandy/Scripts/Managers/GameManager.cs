using System.IO;
using UnityEngine;
using SimpleJSON;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string fileName = "playerPosition.json";

    [SerializeField] AudioSource _gameOverMusic;

    public int difficultyNumber; // 0 = Ez | 1 = Average | 2 = Hard | 3 = Rage
    public int saveNumber;

    public bool loadPositionOnStart;
    public bool isWarping = false;

    public static GameManager Instance;

    private List<ResetCondition> resetConditions = new List<ResetCondition>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            Debug.Log("GameManager instance created and marked as DoNotDestroy.");
        }

        else
        {
            Debug.Log("GameManager instance already exists. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (loadPositionOnStart)
        {
            LoadPlayerPosition();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerCharacter.instance.Restart();
            PlayerUiManager.instance.gameOver.SetActive(false);

            ResetTheme();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            ToggleFullscreen();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("scn_intro");
            Destroy(this.gameObject);
        }
    }

    private void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void FindAllResetobjects()
    {
        // Find all instances of ResetCondition and add them to the list
        ResetCondition[] resetConditionArray = FindObjectsOfType<ResetCondition>();
        resetConditions.AddRange(resetConditionArray);
    }

    public void ResetObjects()
    {
        foreach (ResetCondition resetCondition in resetConditions)
        {
            resetCondition.ResetObjectState();
        }
    }

    void ResetTheme()
    {
        StopAllCoroutines();

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResetMusicPitch();
        }

        _gameOverMusic.Stop();
    }

    public void GameOver()
    {
        if (MusicManager.Instance != null) 
        {
            MusicManager.Instance.LowerPitch();
        }
        PlayerUiManager.instance.GameOver();
        _gameOverMusic.Play();
    }

    public void SavePlayerPosition()
    {
        // Create JSON object
        JSONObject playerData = new JSONObject();
        playerData["position"]["x"].AsFloat = PlayerCharacter.instance.transform.position.x;
        playerData["position"]["y"].AsFloat = PlayerCharacter.instance.transform.position.y;
        playerData["difficulty"].AsInt = difficultyNumber;

        // Get the file path in StreamingAssets
        string filePath = (Application.persistentDataPath + "/playerData" + saveNumber + ".json");

        // Save current scene name
        playerData["sceneName"] = SceneManager.GetActiveScene().name;

        // Write JSON data to the file
        File.WriteAllText(filePath, playerData.ToString());

        Debug.Log("Player position saved!");
    }

    public void LoadPlayerPosition()
    {
        // Get the file path in StreamingAssets
        string filePath = (Application.persistentDataPath + "/playerData" + saveNumber + ".json");

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read JSON data from the file
            string jsonData = File.ReadAllText(filePath);

            // Parse JSON data
            JSONObject playerData = JSON.Parse(jsonData) as JSONObject;

            // Check if the scene name matches the current open scene
            string savedSceneName = playerData["sceneName"];
            string currentSceneName = SceneManager.GetActiveScene().name;
            difficultyNumber = playerData["difficulty"];

            if (savedSceneName != currentSceneName)
            {
                // Load the saved scene before proceeding to load data
                loadPositionOnStart = true;
                resetConditions.Clear();
                PlayerCharacter.instance.StopPlayerAudio();
                SceneManager.LoadScene(savedSceneName);
            }

            // Retrieve player position
            float x = playerData["position"]["x"].AsFloat;
            float y = playerData["position"]["y"].AsFloat;

            // Set player position
            if (savedSceneName == currentSceneName)
            {
                PlayerCharacter.instance.lastSavedPostion = new Vector2(x, y);
            }

            Debug.Log("Player position loaded: " + PlayerCharacter.instance.transform.position);

            Invoke("PositionOnStartResetDelay", 1);
        }

        else
        {
            Debug.Log("No saved player data found. Using default position.");
        }
    }

    void PositionOnStartResetDelay()
    {
        loadPositionOnStart = false;
    }
}