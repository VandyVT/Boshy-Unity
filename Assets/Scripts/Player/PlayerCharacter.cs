using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Player Sounds")]
    [SerializeField] AudioSource _playerAudio;
    [SerializeField] AudioSource _sfxAudio;
    [SerializeField] AudioClip _introClip;
    [SerializeField] AudioClip[] _deathClips;
    [SerializeField] AudioClip _slashClip;
    [SerializeField] AudioClip[] _jumpClips;
    [SerializeField] AudioClip _shootSound;

    [Header("Player Bools")]
    private bool _playerDead = false;

    [Header("Player Sprite")]
    [SerializeField] GameObject _playerSprite;

    [Header("Related Player Objects")]
    [SerializeField] GameObject _bloodPrefab;
    [SerializeField] GameObject _bulletPrefab; // Assign your prefab in the Unity Editor
    GameObject bloodInstance;

    [Header("Player Values")]
    public float moveSpeed = 5f;
    public float maxFallVelocity = -10f;
    public AnimationCurve jumpCurve;
    public AnimationCurve jump2Curve;
    public Animator animator;

    private float bulletSpeed = 8; // Adjust the bullet speed as needed
    private int maxBullets = 5;
    public float bulletLifetime = 2.0f;
    private int currentBulletCount = 0;

    public LayerMask groundLayer;  // You may need to adjust the ground layer

    public string[] deathTexts;
    public TextMeshPro deathText;
    [SerializeField] Transform deathTextPos;

    bool onPlatform;
    bool owater;
    bool djump;
    bool grounded;

    Rigidbody2D _rb;
    [HideInInspector] public Vector2 lastSavedPostion;

    public static PlayerCharacter instance;
    private List<GameObject> bullets = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        djump = true;
        if (_playerAudio == null)
        {
            _playerAudio = GetComponent<AudioSource>();
        }

        else
        {
            // Random speed and pitch within a specified range
            float randomSpeed = Random.Range(0.75f, 1.25f);

            // Set the pitch and play the audio clip
            _playerAudio.pitch = randomSpeed;
            _playerAudio.PlayOneShot(_introClip);
        }

        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        deathText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerDead();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot();
        }

        CheckDestroyedBullets();
        UpdateAnimation();
        Move();
        Jump();
    }

    void FixedUpdate()
    {
        // Check if the player is falling
        if (_rb.velocity.y < maxFallVelocity)
        {
            // Set the y component of the velocity to the maximum fall velocity
            _rb.velocity = new Vector3(_rb.velocity.x, maxFallVelocity);
        }
    }

    void UpdateAnimation()
    {
        // Check if the "Walk" animation bool should be true
        bool isWalking = Mathf.Abs(Input.GetAxis("Horizontal")) > 0;

        // Set the "Walk" animation bool in the Animator
        animator.SetBool("Walk", isWalking);

        // Check if the player is jumping or falling
        bool isJumping = _rb.velocity.y > 0;
        bool isFalling = _rb.velocity.y < 0;

        // Set the "Jump" and "Fall" animation bools in the Animator
        animator.SetBool("Jump", isJumping);
        animator.SetBool("Fall", isFalling);
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Set the velocity directly based on the input and desired constant speed
        if (moveInput != 0)
        {
            _rb.velocity = new Vector2(moveSpeed * Mathf.Sign(moveInput), _rb.velocity.y);
        }

        else if (moveInput == 0)
        {
            // Stop the player when no input is detected
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        if (_rb.velocity.x >= 1)
        {
            // If the velocity is positive or zero, keep the original scale
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (_rb.velocity.x < -1)
        {
            // If the velocity is negative, flip the object along the X-axis
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    void Jump()
    {
        Vector2 boxSize = new Vector2(0.25f, 0.2f);
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y - 0.05f);

        // Perform a 2D boxcast to check for the "Ground" layer
        grounded = Physics2D.BoxCast(boxPosition, boxSize, 0f, Vector2.down, 0.1f, groundLayer);

        if (grounded)
        {
            djump = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (grounded || onPlatform || Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask.GetMask("Water")))
            {
                // Regular jump
                if (!owater)
                {
                    PlaySound("jump1");
                }

                if (owater)
                {
                    djump = !Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask.GetMask("WaterPlatform"));
                }
                else
                {
                    djump = true;
                }

                float jumpForce = jumpCurve.Evaluate(Time.time); // Use the Animation Curve for regular jump
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            }

            else if (djump || Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask.GetMask("Water2")))
            {
                // Double jump
                float jump2Force = jump2Curve.Evaluate(Time.time); // Use the Animation Curve for double jump
                _rb.velocity = new Vector2(_rb.velocity.x, jump2Force);
                PlaySound("jump2");
                djump = false;
            }
        }

        // Negate vertical velocity after releasing the jump key
        if (Input.GetKeyUp(KeyCode.Z) && _rb.velocity.y > 0)
        {
            // Adjust the value based on your needs
            float maxFallSpeed = 5.0f;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, 0, maxFallSpeed));
        }
    }

    public void EnableExtraJump()
    {
        djump = true;
    }

    void Shoot()
    {
        if (bullets.Count < maxBullets)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            _sfxAudio.PlayOneShot(_shootSound);

            // Check the local scale to determine the direction
            float direction = transform.localScale.x;

            // Set the velocity based on the direction
            bulletRb.velocity = new Vector2(direction, 0) * bulletSpeed;

            // Automatically destroy the bullet after a certain lifetime
            Destroy(bullet, bulletLifetime);

            // Add the bullet to the list
            bullets.Add(bullet);
        }
    }

    void CheckDestroyedBullets()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (bullets[i] == null)
            {
                // Bullet was destroyed, remove from the list
                bullets.RemoveAt(i);
                BulletDestroyed();
            }
        }
    }

    void BulletDestroyed()
    {
        // Logic to handle a destroyed bullet
        currentBulletCount--;
    }

    public void DestroyAllBullets()
    {
        foreach (GameObject obj in bullets)
        {
            // Check if the object exists before attempting to destroy it
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // Clear the list after destroying all objects
        bullets.Clear();
    }

    void PlaySound(string soundName)
    {
        if (_sfxAudio == null)
        {
            Debug.LogError("SFX Audio source not assigned.");
            return;
        }

        switch (soundName)
        {
            case "jump1":
                _sfxAudio.PlayOneShot(_jumpClips[0]);
                break;
            case "jump2":
                _sfxAudio.PlayOneShot(_jumpClips[1]);
                break;
            // Add more cases for other sound names if needed
            default:
                Debug.LogWarning("Unknown sound name: " + soundName);
                break;
        }
    }

    bool PlayerDead()
    {
        if (_playerDead == false)
        {
            OnPlayerDeath();
            _playerDead = true;
            Debug.Log("Player Died? " + _playerDead);
        }

        return _playerDead;
    }

    void OnPlayerDeath()
    {
        if (_playerDead) return;

        _sfxAudio.PlayOneShot(_slashClip);

        // Random speed and pitch within a specified range
        float randomSpeed = Random.Range(0.75f, 1.25f);

        // Get a random index within the array length
        int randomIndex = Random.Range(0, deathTexts.Length);

        // Select the random text
        string randomText = deathTexts[randomIndex];

        // Set the pitch and play the audio clip
        _playerAudio.pitch = randomSpeed;
        _playerAudio.PlayOneShot(_deathClips[randomIndex]); // Assuming _deathClips is your array of AudioClips

        if (_bloodPrefab != null)
        {
            // Spawn the prefab at the current position and rotation of the script's GameObject
            bloodInstance = Instantiate(_bloodPrefab, transform.position, transform.rotation);
        }

        // Set TMPro text to the selected string
        deathText.text = randomText;
        deathText.transform.position = deathTextPos.position;

        // Start coroutine for the shake effect
        StartCoroutine(ShakeText());

        // Other game over actions
        GameManager.Instance.GameOver();

        _playerSprite.SetActive(false);
        this.GetComponent<BoxCollider2D>().enabled = false;
        _rb.bodyType = RigidbodyType2D.Static;
        PlayerCharacter playerScript = this.GetComponent<PlayerCharacter>();
        playerScript.enabled = false;
    }

    IEnumerator ShakeText()
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float slowdownFactor = 1f;

        while (elapsedTime < duration)
        {
            float offsetY = Random.Range(-0.5f, 0.5f);

            float lerpFactor = 1 - elapsedTime / duration;
            Vector3 shakeOffset = new Vector3(0, offsetY, 0f) * slowdownFactor * lerpFactor;

            deathText.transform.position = deathTextPos.position + shakeOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // After the shake, wait for 1.5 seconds and set text to nothing
        yield return new WaitForSeconds(1f);
        deathText.text = "";
        deathText.transform.position = deathTextPos.position; // Reset the position
    }

    public void Restart()
    {
        djump = true;
        _playerDead = false;

        GameManager.Instance.LoadPlayerPosition();
        // Random speed and pitch within a specified range
        float randomSpeed = Random.Range(0.75f, 1.25f);

        // Set the pitch and play the audio clip
        _playerAudio.pitch = randomSpeed;
        _playerAudio.Stop();
        _playerAudio.PlayOneShot(_introClip);

        transform.position = lastSavedPostion;
        _playerSprite.SetActive(true);
        this.GetComponent<BoxCollider2D>().enabled = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.velocity = Vector2.zero;

        PlayerCharacter playerScript = this.GetComponent<PlayerCharacter>();
        playerScript.enabled = true;

        deathText.text = "";
        StopCoroutine(ShakeText());

        Destroy(bloodInstance);
        DestroyAllBullets();

        grounded = Physics2D.Raycast(transform.position, Vector2.down, 0.25f, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Spike")
        {
            OnPlayerDeath();
        }

        if (collision.name == "SaveSpit")
        {
            OnPlayerDeath();
        }

        if (collision.name == ("Speed Forward"))
        {
            // Increase the player's right speed when standing on the trigger
            moveSpeed *= 1.5f;  // You can adjust the multiplier as needed
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == ("Speed Forward"))
        {
            // Reset the player's right speed when leaving the trigger
            moveSpeed /= 1.5f;  // You can adjust the divisor as needed
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == ("Speed Forward"))
        {
            _rb.velocity = new Vector2(moveSpeed, _rb.velocity.y);
        }
    }
}