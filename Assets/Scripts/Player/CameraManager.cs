using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject playerObject;
    public float moveAmountX = 13.35f;
    public float moveAmountY = 10f;

    void Update()
    {
        if (playerObject != null)
        {
            Vector2 objectPosition = Camera.main.WorldToViewportPoint(playerObject.transform.position);

            if (objectPosition.x < 0 || objectPosition.x > 1 || objectPosition.y < 0 || objectPosition.y > 1)
            {
                MoveCamera(objectPosition);
            }
        }
    }

    void MoveCamera(Vector3 objectPosition)
    {
        Vector3 cameraPosition = transform.position;

        if (objectPosition.x < 0)
        {
            cameraPosition.x -= moveAmountX;
        }
        else if (objectPosition.x > 1)
        {
            cameraPosition.x += moveAmountX;
        }

        if (objectPosition.y < 0)
        {
            cameraPosition.y -= moveAmountY;
        }
        else if (objectPosition.y > 1)
        {
            cameraPosition.y += moveAmountY;
        }

        transform.position = cameraPosition;
    }
}
