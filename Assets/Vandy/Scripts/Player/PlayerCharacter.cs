using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] bool isMenuAnimation;

    [Header("Scene Collection")]
    [SerializeField] Transform playerHolderObject;
    [SerializeField] Transform regularPlayerCollection;

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
    [SerializeField] GameObject _bulletPrefab;
    GameObject bloodInstance;

    [Header("Player Values")]
    float moveSpeed = 3.25f;
    float maxFallVelocity = -7.5f;
    float waterFallVelocity = -2f;
    float vineFallVelocity = -1f;
    float jumpCurve = 9.50f;
    float jump2Curve = 7.25f;
    public Animator animator;

    float bulletSpeed = 12;
    int maxBullets = 5;
    float bulletLifetime = 1f;
    int currentBulletCount = 0;

    public LayerMask groundLayer;

    [SerializeField] string[] deathTexts;
    [SerializeField] TextMeshPro deathText;
    [SerializeField] Transform deathTextPos;

    bool onPlatform;
    bool onWater;
    bool onVine;
    bool djump;
    bool grounded;

    Rigidbody2D _rb;
    [HideInInspector] public Vector2 lastSavedPostion;

    public static PlayerCharacter instance;
    private List<GameObject> bullets = new List<GameObject>();

    private void Awake()
    {
        instance = this;
        
        if (_rb == null)
        {
            if (isMenuAnimation) return;
            _rb = GetComponent<Rigidbody2D>();
        }

        if (GameManager.Instance.loadPositionOnStart)
        {
            _playerAudio.Stop();
            Restart();
        }
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
            float randomSpeed = Random.Range(0.75f, 1.25f);

            _playerAudio.pitch = randomSpeed;
            _playerAudio.PlayOneShot(_introClip);
        }

        GameManager.Instance.FindAllResetobjects();
        deathText.text = "";
    }

    void Update()
    {
        if (isMenuAnimation || GameManager.Instance.isWarping) return;

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
        if (isMenuAnimation) return;

        if (onWater)
        {
            if (_rb.velocity.y < waterFallVelocity)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, waterFallVelocity);
            }
        }

        if (onVine)
        {
            if (_rb.velocity.y < vineFallVelocity)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, vineFallVelocity);
            }
        }

        else if(_rb.velocity.y < maxFallVelocity)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, maxFallVelocity);
        }
    }

    void UpdateAnimation()
    {
        // Check if the "Walk" animation bool should be true
        bool isWalking = Mathf.Abs(Input.GetAxis("Horizontal")) > 0;

        // Set the "Walk" animation bool in the Animator
        WalkAnimation(isWalking);

        // Check if the player is jumping or falling
        bool isJumping = _rb.velocity.y > 0;
        bool isFalling = _rb.velocity.y < 0;

        // Set the "Jump" and "Fall" animation bools in the Animator
        JumpAnimation(isJumping);
        FallAnimation(isFalling);
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

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
                // First jump
                PlaySound("jump1");

                float jumpForce = jumpCurve;
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            }

            else if (djump || Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask.GetMask("Water2")))
            {
                // Double jump
                PlaySound("jump2");

                float jump2Force = jump2Curve;
                _rb.velocity = new Vector2(_rb.velocity.x, jump2Force);
                djump = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z) && _rb.velocity.y > 0)
        {
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

            float direction = transform.localScale.x;

            bulletRb.velocity = new Vector2(direction, 0) * bulletSpeed;

            Destroy(bullet, bulletLifetime);

            bullets.Add(bullet);
        }
    }

    void CheckDestroyedBullets()
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            if (bullets[i] == null)
            {
                bullets.RemoveAt(i);
                BulletDestroyed();
            }
        }
    }

    void BulletDestroyed()
    {
        currentBulletCount--;
    }

    public void DestroyAllBullets()
    {
        foreach (GameObject obj in bullets)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

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
    private bool bloodInstantiated = false; 

    void OnPlayerDeath()
    {
        if (_playerDead) return;

        _sfxAudio.PlayOneShot(_slashClip);

        float randomSpeed = Random.Range(0.75f, 1.25f);

        int randomIndex = Random.Range(0, deathTexts.Length);

        string randomText = deathTexts[randomIndex];

        _playerAudio.pitch = randomSpeed;
        _playerAudio.PlayOneShot(_deathClips[randomIndex]);

        if (_bloodPrefab != null && !bloodInstantiated)
        {
            bloodInstance = Instantiate(_bloodPrefab, transform.position, transform.rotation);
            bloodInstantiated = true; 
        }

        deathText.text = randomText;
        deathText.transform.position = deathTextPos.position;

        StartCoroutine(ShakeText());

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
        float duration = 0.4f;
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

        yield return new WaitForSeconds(1f);
        deathText.text = "";
        deathText.transform.position = deathTextPos.position;
    }

    public void Restart()
    {
        if (isMenuAnimation || GameManager.Instance.isWarping) return;

        bloodInstantiated = false;
        djump = true;
        onWater = false;
        _playerDead = false;

        GameManager.Instance.LoadPlayerPosition();
        GameManager.Instance.ResetObjects();

        // Random speed and pitch within a specified range
        float randomSpeed = Random.Range(0.75f, 1.25f);

        // Set the pitch and play the audio clip
        _playerAudio.pitch = randomSpeed;
        _playerAudio.Stop();
        _playerAudio.PlayOneShot(_introClip);

        Debug.Log("Restart Triggered");

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
        if (collision.name == "Spike" || collision.name == "Fruit" || collision.name == "SaveSpit")
        {
            OnPlayerDeath();
        }

        if (collision.name == ("Speed Forward"))
        {
            // Increase the player's right speed when standing on the trigger
            moveSpeed *= 1.5f;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == ("Speed Forward"))
        {
            _rb.velocity = new Vector2(moveSpeed, _rb.velocity.y);
        }

        if (collision.name == ("Water"))
        {
            djump = true;
            onWater = true;
        }

        if (collision.name == ("Vine"))
        {
            onVine = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == ("Speed Forward"))
        {
            // Reset the player's right speed when leaving the trigger
            moveSpeed /= 1.5f;
        }

        if (collision.name == ("Water"))
        {
            onWater = false;
        }

        if (collision.name == ("Vine"))
        {
            onVine = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Player is on a moving platform, set it as a child
            playerHolderObject.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Player is no longer on the moving platform, set it back to sceneCollectionObj
            playerHolderObject.SetParent(regularPlayerCollection);
        }
    }

    public void WalkAnimation(bool isWalking)
    {
        animator.SetBool("Walk", isWalking);
    }

    public void JumpAnimation(bool isJumping)
    {
        animator.SetBool("Jump", isJumping);
    }

    public void FallAnimation(bool isFalling)
    {
        animator.SetBool("Fall", isFalling);
    }
}