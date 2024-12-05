using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect component
    [SerializeField] private RectTransform content; // The content RectTransform of the ScrollRect

    private void Update()
    {
        // Get the currently selected GameObject
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null || !selected.transform.IsChildOf(content)) return;

        RectTransform selectedRect = selected.GetComponent<RectTransform>();
        if (selectedRect == null) return;

        // Check if the selected item is out of the visible area
        Vector3[] viewportCorners = new Vector3[4];
        scrollRect.viewport.GetWorldCorners(viewportCorners);

        Vector3[] selectedCorners = new Vector3[4];
        selectedRect.GetWorldCorners(selectedCorners);

        // Check if the selected item is outside the viewport
        bool isAbove = selectedCorners[1].y > viewportCorners[1].y;
        bool isBelow = selectedCorners[0].y < viewportCorners[0].y;

        if (isAbove || isBelow)
        {
            // Calculate the target scroll position
            float contentHeight = content.rect.height;
            float viewportHeight = scrollRect.viewport.rect.height;

            // Get the position of the selected element relative to the content
            float selectedPosition = contentHeight - selectedRect.anchoredPosition.y;

            // Calculate the normalized scroll position
            float normalizedPosition = (selectedPosition - viewportHeight / 2) / (contentHeight - viewportHeight);
            normalizedPosition = Mathf.Clamp01(normalizedPosition);

            // Set the scroll position
            scrollRect.verticalNormalizedPosition = 1 - normalizedPosition;
        }
    }
}