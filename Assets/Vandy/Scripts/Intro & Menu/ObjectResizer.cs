using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectResizer : MonoBehaviour
{
    private Vector3 initialSize;

    [SerializeField] private AnimationCurve resizeCurve;

    private void Awake()
    {
        // Get the initial size of the GameObject
        initialSize = transform.localScale;
    }

    // Coroutine to scale up the GameObject based on a multiplier
    private IEnumerator ScaleUp(float multiplier, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float progress = timer / duration;
            float curveValue = resizeCurve.Evaluate(progress); // Use the curve to ease the scaling
            transform.localScale = Vector3.Lerp(initialSize, initialSize * multiplier, curveValue);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialSize * multiplier;
    }

    // Coroutine to scale down the GameObject to its original size
    private IEnumerator ScaleDown(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float progress = timer / duration;
            float curveValue = resizeCurve.Evaluate(progress); 
            transform.localScale = Vector3.Lerp(transform.localScale, initialSize, curveValue);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = initialSize;
    }

    // Public function to be called on Event Trigger
    public void Resize(bool scaleUp)
    {
        if (scaleUp)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleUp(1.4f, 0.2f));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ScaleDown(0.2f));
        }
    }

    private void OnDisable()
    {
        transform.localScale = initialSize;
    }
}
