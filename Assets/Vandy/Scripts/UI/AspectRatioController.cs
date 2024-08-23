using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class AspectRatioController : MonoBehaviour
{
    public Canvas canvas;

    void Start()
    {
        AdjustCanvas();
    }

    void OnRectTransformDimensionsChange()
    {
        AdjustCanvas();
    }

    void AdjustCanvas()
    {
        // The aspect ratio to maintain
        float targetAspectRatio = 4.0f / 3.0f;

        // Get the current screen's aspect ratio
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the scale factor based on the aspect ratio difference
        if (screenAspectRatio >= targetAspectRatio)
        {
            float scaleFactor = targetAspectRatio / screenAspectRatio;
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0; // Match width
            canvas.GetComponent<RectTransform>().localScale = new Vector3(scaleFactor, 1, 1);
        }
        else
        {
            float scaleFactor = screenAspectRatio / targetAspectRatio;
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1; // Match height
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, scaleFactor, 1);
        }

        AdjustChildObjects();
    }

    void AdjustChildObjects()
    {
        // The aspect ratio you want to maintain
        float targetAspectRatio = 4.0f / 3.0f;

        // Get the current screen's aspect ratio
        float screenAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the Y scale adjustment based on the aspect ratio difference
        float yScale = targetAspectRatio / screenAspectRatio;

        // Iterate through each child of the canvas and adjust its scale
        foreach (RectTransform child in canvas.GetComponent<RectTransform>())
        {
            // Ignore the child named "Game Renderer"
            if (child.name == "Game Renderer")
                continue;

            if (child.name == "Vignette")
                continue;

            // Set the scale of child objects to maintain their proportions
            if (screenAspectRatio == targetAspectRatio)
            {
                // If the screen is exactly 4:3, keep the scale at 1
                child.localScale = Vector3.one;
            }
            else if (screenAspectRatio > targetAspectRatio)
            {
                // If the screen is wider than 4:3, adjust Y scale only
                child.localScale = new Vector3(1, yScale, 1);
            }
            else
            {
                // If the screen is taller than 4:3, adjust X scale only
                child.localScale = new Vector3(yScale, 1, 1);
            }
        }
    }
}
