using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHellKitty : MonoBehaviour
{
    [SerializeField] AudioSource introAudioSource;
    [SerializeField] AudioSource bossTheme;
    [SerializeField] AudioSource bossSfx;
    [SerializeField] AudioClip hateClip;
    [SerializeField] AudioClip roarClip;
    [SerializeField] AudioClip dmgSound;
    [SerializeField] AudioClip dyingCharge;
    [SerializeField] SpriteRenderer introBG;

    [SerializeField] GameObject bossBG;

    [Header("Animations")]
    [SerializeField] Animator helloKitSmall;
    [SerializeField] Animator hellKitBig;
    [SerializeField] Animator platforms;

    [Header("Boss Parameters")]
    [SerializeField] int bossHealth;
    [SerializeField] Slider bossHpSlider;
    [SerializeField] SpriteRenderer bossSpr;

    private bool canSkip = true;
    private bool isActive = false;

    void Start()
    {
        StartCoroutine(IntroSequence());
        GetBossDifficulty();
        bossSpr.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            StopAllCoroutines();
            ActivateBoss();
        }
    }

    IEnumerator IntroSequence()
    {
        // Initial wait period
        yield return new WaitForSeconds(1.5f); // Adjust the time as needed

        // Play intro audio && Fade in background 
        StartCoroutine(FadeIntroBG());
        helloKitSmall.Play("Intro");
        introAudioSource.Play();

        // Wait for 30 seconds
        yield return new WaitForSeconds(25f); // Adjust the time as needed

        // Play sound clip
        introAudioSource.PlayOneShot(hateClip, 0.4f);

        // Wait for a couple more seconds
        yield return new WaitForSeconds(2.75f); // Adjust the time as needed

        // End period, activate the boss
        ActivateBoss();
    }

    private IEnumerator FadeIntroBG()
    {
        float elapsedTime = 0f;
        Color startColor = introBG.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < 0.25f)
        {
            introBG.color = Color.Lerp(startColor, targetColor, elapsedTime / 0.25f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha is exactly 1
        introBG.color = targetColor;
    }

    void GetBossDifficulty()
    {
        if (GameManager.Instance != null)
        {
            int getHp = GameManager.Instance.difficultyNumber;

            if (getHp == 0)
            {
                bossHealth = 50;
            }

            if (getHp == 1)
            {
                bossHealth = 100;
            }

            if (getHp == 2)
            {
                bossHealth = 100;
            }

            if (getHp == 3)
            {
                bossHealth = 200;
            }

            if (bossHpSlider != null)
            {
                bossHpSlider.maxValue = bossHealth;
                bossHpSlider.value = bossHealth;
            }
        }
    }

    void ActivateBoss()
    {
        if (!canSkip) return;
        introAudioSource.Stop();
        bossTheme.Play();
        introAudioSource.PlayOneShot(roarClip, 0.5f);
        bossBG.SetActive(true);
        bossSpr.enabled = true;
        helloKitSmall.gameObject.SetActive(false);
        platforms.Play("Platforms");
        CameraManager.instance.Shake(0.8f, 0.8f, 0.2f);
        canSkip = false;
        isActive = true;
    }

    void TakeHit()
    {
        bossHealth = bossHealth - 1;
        bossHpSlider.value = bossHealth;
        bossSfx.PlayOneShot(dmgSound);

        if (bossHealth == 0)
        {
            StartCoroutine(EndSequence());
        }
    }
    IEnumerator EndSequence()
    {
        // Initial wait period
        yield return new WaitForSeconds(1f); // Adjust the time as needed
        bossSfx.PlayOneShot(dyingCharge, 0.25f);

        // Stop Music after 4 more seconds
        yield return new WaitForSeconds(4f);
        bossTheme.Stop();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Bullet")
        {
            if (!isActive) return;
            if (bossHealth != 0)
            {
                TakeHit();
                Destroy(collision.gameObject);
            }
        }
    }
}
