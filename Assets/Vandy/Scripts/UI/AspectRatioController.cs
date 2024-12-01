using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class AspectRatioController : MonoBehaviour
{
    private const float TargetAspectRatio = 4.0f / 3.0f;
    private Canvas canvas;
    private RectTransform canvasRectTransform;

    private bool roomStart = true;

    private void Awake()
    {
        GetCanvasComponents();
    }

    private void Start()
    {
        AdjustCanvas();
        AdjustChildObjects();

        roomStart = false;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (canvasRectTransform != null)
        {
            AdjustCanvas();

            if (roomStart) return;
            AdjustChildObjects();
        }
    }

    private void GetCanvasComponents()
    {
        canvas = GetComponent<Canvas>();

        // If not found on the same GameObject, search in children
        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();
        }

        // Attempt to get RectTransform only if Canvas exists
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        // Log errors for missing components
        if (canvas == null)
        {
            Debug.LogError("AspectRatioController: Canvas not found. Ensure this script is attached to the Canvas GameObject or its parent.");
        }
        if (canvasRectTransform == null)
        {
            Debug.LogError("AspectRatioController: RectTransform not found. Ensure the Canvas GameObject has a RectTransform component.");
        }
    }

    private void AdjustCanvas()
    {
        if (canvasRectTransform == null) return;

        float screenAspectRatio = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(screenAspectRatio, TargetAspectRatio))
        {
            canvasRectTransform.localScale = Vector3.one;
            canvasRectTransform.sizeDelta = Vector2.zero;
            return;
        }

        float scaleFactor = (screenAspectRatio > TargetAspectRatio)
            ? TargetAspectRatio / screenAspectRatio
            : screenAspectRatio / TargetAspectRatio;

        canvasRectTransform.localScale = screenAspectRatio > TargetAspectRatio
            ? new Vector3(scaleFactor, 1, 1)
            : new Vector3(1, scaleFactor, 1);

        canvasRectTransform.anchoredPosition = Vector2.zero;
    }

    private void AdjustChildObjects()
    {
        if (canvasRectTransform == null) return;

        float screenAspectRatio = (float)Screen.width / Screen.height;
        float xScale = (screenAspectRatio > TargetAspectRatio) ? 1 : screenAspectRatio / TargetAspectRatio;
        float yScale = (screenAspectRatio > TargetAspectRatio) ? TargetAspectRatio / screenAspectRatio : 1;

        foreach (RectTransform child in canvasRectTransform)
        {
            if (child.name == "Game Renderer" || child.name == "Vignette") continue;

            // Reset local scale to avoid accumulation errors
            child.localScale = Vector3.one;

            // Calculate new position based on pivot
            Vector2 pivotOffset = new Vector2(
                (child.pivot.x - 0.5f) * child.rect.width * (xScale - 1),
                (child.pivot.y - 0.5f) * child.rect.height * (yScale - 1)
            );

            Vector2 adjustedPosition = new Vector2(
                child.anchoredPosition.x * xScale + pivotOffset.x,
                child.anchoredPosition.y * yScale + pivotOffset.y
            );

            child.anchoredPosition = adjustedPosition;
            child.localScale = new Vector3(xScale, yScale, 1);
        }
    }
}
