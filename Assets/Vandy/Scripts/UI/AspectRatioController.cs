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
    }
}
