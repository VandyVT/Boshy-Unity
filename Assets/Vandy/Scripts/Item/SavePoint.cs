using UnityEngine;
using System.Collections;

public class SavePoint : MonoBehaviour
{
    public AudioSource _saveAudioSource;
    public AudioClip _saveClip;

    private Animator animator;

    [Header("SPIT")]
    public GameObject prefabToSpawn;
    public GameObject prefabSpawned;
    public float destroyDelay = 1f;
    private bool isSaving = false;
    private bool canSpawn = true;
    public Vector2 currentPlayerPos;

    [Header("Difficulty Settings")]
    [Tooltip("Maximum 3, despawns save if despawn difficulty is less than the GameManager's Difficulty, 0 being easiest, 3 being rage mode, set it to 3 if you never want it to despawn on any difficulty")]
    [SerializeField] int despawnDifficulty; 

    private void Start()
    {
        if (despawnDifficulty < GameManager.Instance.difficultyNumber)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Save()
    {
        animator = GetComponent<Animator>();

        currentPlayerPos = PlayerCharacter.instance.transform.position;

        // Random speed and pitch within a specified range
        float randomSpeed = Random.Range(0.75f, 1.25f);

        // Set the pitch and play the audio clip
        _saveAudioSource.Stop();
        _saveAudioSource.pitch = randomSpeed;
        _saveAudioSource.PlayOneShot(_saveClip);

        animator.Play("Save");
        if (!isSaving)
        { 
            Invoke("SpitOnPlayer", 0.5f);
        }

        isSaving = true;

        GameManager.Instance.SavePlayerPosition();
    }

    void SpitOnPlayer()
    {
        canSpawn = false;

        // Spawn the prefab at its initial position
        prefabSpawned = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        prefabSpawned.name = prefabToSpawn.name;

        // Get the initial position of the spawned prefab
        Vector2 initialPosition = prefabSpawned.transform.position;

        // Store the player character's position
        currentPlayerPos = PlayerCharacter.instance.transform.position;

        // Calculate the force needed to reach the player character position
        Vector2 force = CalculateForce(initialPosition, currentPlayerPos);

        // Apply force to the Rigidbody2D
        prefabSpawned.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

        // Destroy the spawned prefab after "destroyedDelay" time has passed
        Invoke("WaitUntilDestroy", destroyDelay);
    }

    void WaitUntilDestroy()
    {
        Destroy(prefabSpawned);
        canSpawn = true;
        isSaving = false;
    }

    Vector2 CalculateForce(Vector3 currentPos, Vector3 targetPos)
    {
        // Calculate the direction towards the target
        Vector2 direction = new Vector2(targetPos.x - currentPos.x, targetPos.y - currentPos.y);

        // Add a slight force on the Y-axis
        direction.y += 0.5f;

        // Normalize the direction and scale it by a desired force magnitude
        Vector2 force = direction.normalized * 6f; 

        return force;
    }
}
