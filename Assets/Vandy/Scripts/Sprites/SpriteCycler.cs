using System.Collections;
using UnityEngine;

public class SpriteCycler : MonoBehaviour
{
    [Header("Sprite Settings")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    [Header("Cycle Settings")]
    public float cycleTime = 1.0f;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (sprites.Length == 0)
        {
            Debug.LogError("Sprite array is empty. Please assign sprites in the inspector.");
            return;
        }

        StartCoroutine(CycleSprites());
    }

    private IEnumerator CycleSprites()
    {
        int currentIndex = 0;

        while (true)
        {
            spriteRenderer.sprite = sprites[currentIndex];

            yield return new WaitForSeconds(cycleTime);

            currentIndex = (currentIndex + 1) % sprites.Length;
        }
    }
}
