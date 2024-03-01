using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCycler : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Component spriteComponent;
    public Object[] sprites;

    [Header("Cycle Settings")]
    public float cycleTime = 1.0f;
    public bool cycleOnce = false;

    private Coroutine cycleCoroutine;

    private void Start()
    {
        if (spriteComponent == null)
        {
            // Try to get the component from the GameObject
            spriteComponent = GetComponent<SpriteRenderer>();
            if (spriteComponent == null)
            {
                spriteComponent = GetComponent<Image>();
                if (spriteComponent == null)
                {
                    spriteComponent = GetComponent<RawImage>();
                    if (spriteComponent == null)
                    {
                        Debug.LogError("No SpriteRenderer, Image, or RawImage component found. Please assign a valid component in the inspector.");
                        return;
                    }
                }
            }
        }

        if (sprites.Length == 0)
        {
            Debug.LogError("Sprite array is empty. Please assign sprites in the inspector.");
            return;
        }

        StartCoroutine(CycleSprites());
    }

    private void OnEnable()
    {
        // Restart the coroutine when the object is enabled
        if (cycleCoroutine == null)
        {
            cycleCoroutine = StartCoroutine(CycleSprites());
        }
    }

    private void OnDisable()
    {
        // Stop the coroutine when the object is disabled
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
        }
    }

    private IEnumerator CycleSprites()
    {
        int currentIndex = 0;
        int totalSprites = sprites.Length;

        while (true)
        {
            if (spriteComponent is SpriteRenderer)
            {
                ((SpriteRenderer)spriteComponent).sprite = (Sprite)sprites[currentIndex];
            }
            else if (spriteComponent is Image)
            {
                ((Image)spriteComponent).sprite = (Sprite)sprites[currentIndex];
            }
            else if (spriteComponent is RawImage)
            {
                ((RawImage)spriteComponent).texture = (Texture)sprites[currentIndex];
            }

            yield return new WaitForSeconds(cycleTime);

            if (cycleOnce && currentIndex == totalSprites - 1)
            {
                yield break; 
            }

            currentIndex = (currentIndex + 1) % totalSprites;
        }
    }
}
