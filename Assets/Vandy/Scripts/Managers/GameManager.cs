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
    private bool cameFromMainMenu = false;

    public static GameManager Instance;

    private List<ResetCondition> resetConditions = new List<ResetCondition>();

    [SerializeField] GameObject rendererPrefab;
    [SerializeField] GameObject borderPrefab;

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

        if (!RendererExists())
        {
            // Instantiate the prefab at the desired position and rotation
            Instantiate(rendererPrefab, Vector3.zero, Quaternion.identity);
        }

        if (!BorderExists())
        {
            // Instantiate the prefab at the desired position and rotation
            Instantiate(borderPrefab, Vector3.zero, Quaternion.identity);
        }

        string previousScene = PlayerPrefs.GetString("PreviousScene", "");
        if (previousScene == "scn_mainMenu")
        {
            cameFromMainMenu = true;
        }
    }

    bool RendererExists()
    {
        GameObject existingPrefab = GameObject.Find(rendererPrefab.name + "(Clone)");
        return existingPrefab != null;
    }

    bool BorderExists()
    {
        GameObject existingPrefab = GameObject.Find(borderPrefab.name + "(Clone)");
        return existingPrefab != null;
    }

    private void Update()
    {
        if (PlayerInputs.instance.resetAction.triggered)
        {
            PlayerCharacter.instance.Restart();
            PlayerUiManager.instance.gameOver.SetActive(false);

            ResetTheme();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            ResetGame();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            ToggleFullscreen();
        }

        if (PlayerInputs.instance.optionsAction.triggered)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == "scn_mainMenu")
            {
                return;
            }

            else if (currentSceneName == "scn_options")
            {
                if (cameFromMainMenu)
                {
                    ResetGame();
                }
                else
                {
                    LoadPlayerPosition();
                }
            }
            else
            {
                PlayerPrefs.SetString("PreviousScene", currentSceneName);
                SceneManager.LoadScene("scn_options");
            }
        }
    }

    void ResetGame()
    {
        SceneManager.LoadScene("scn_intro");
        Destroy(this.gameObject);

        if (RendererExists())
        {
            // Instantiate the prefab at the desired position and rotation
            Destroy(GameObject.Find(rendererPrefab.name + "(Clone)"));
        }

        if (BorderExists())
        {
            // Instantiate the prefab at the desired position and rotation
            Destroy(GameObject.Find(borderPrefab.name + "(Clone)"));
        }
    }

    private void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ToggleOptionsFromMenu(bool Toggle)
    {
        cameFromMainMenu = Toggle;
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
        ToggleOptionsFromMenu(false);

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
                if (PlayerCharacter.instance != null)
                {
                    PlayerCharacter.instance.StopPlayerAudio();
                }
                SceneManager.LoadScene(savedSceneName);
            }

            // Retrieve player position
            float x = playerData["position"]["x"].AsFloat;
            float y = playerData["position"]["y"].AsFloat;

            // Set player position
            if (savedSceneName == currentSceneName && PlayerCharacter.instance != null)
            {
                PlayerCharacter.instance.lastSavedPostion = new Vector2(x, y);
            }

            if (PlayerCharacter.instance != null)
            {
                Debug.Log("Player position loaded: " + PlayerCharacter.instance.transform.position);
            }

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