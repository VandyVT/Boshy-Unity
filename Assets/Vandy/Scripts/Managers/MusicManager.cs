using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] AudioSource[] _levelMusic;
    [SerializeField] bool playMusicOnStart; 

    float fadeTime = 3.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null && !audioSource.isPlaying && playMusicOnStart)
            {
                audioSource.Play();
            }
        }
    }

    public void ResetMusicPitch()
    {
        CancelPitch();

        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null)
            {
                audioSource.pitch = 1f;
            }
        }
    }

    public void LowerPitch()
    {
        StartCoroutine(FadeThemePitch());
    }

    public void CancelPitch()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeThemePitch()
    {
        float currentTime = 0;
        float initialPitch = _levelMusic[0].pitch;

        while (currentTime < fadeTime)
        {
            float normalizedTime = currentTime / fadeTime;
            foreach (var audioSource in _levelMusic)
            {
                if (audioSource != null)
                {
                    audioSource.pitch = Mathf.Lerp(initialPitch, 0f, normalizedTime);
                }
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        foreach (var audioSource in _levelMusic)
        {
            if (audioSource != null)
            {
                audioSource.pitch = 0f;
            }
        }
    }
}
