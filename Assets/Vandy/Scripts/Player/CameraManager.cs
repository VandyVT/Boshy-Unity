using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public GameObject playerObject;
    public float moveAmountX = 13.15f;
    public float moveAmountY = 10f;

    private Transform cameraTransform;
    private Vector3 originalPosition;

    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
        cameraTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

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

    public void Shake(float shakeAmountX, float shakeAmountY, float duration)
    {
        if (playerObject != null)
        {
            Vector2 objectPosition = Camera.main.WorldToViewportPoint(playerObject.transform.position);

            if (objectPosition.x < 0 || objectPosition.x > 1 || objectPosition.y < 0 || objectPosition.y > 1)
            {
                MoveCamera(objectPosition);
            }
        }

        originalPosition = cameraTransform.position;
        StartCoroutine(ShakeCoroutine(shakeAmountX, shakeAmountY, duration));
    }

    private IEnumerator ShakeCoroutine(float shakeAmountX, float shakeAmountY, float duration)
    {
        float elapsedTime = 0f;
        float slowdownFactor = 1f;

        while (elapsedTime < duration)
        {
            float offsetX = Random.Range(-shakeAmountX, shakeAmountX);
            float offsetY = Random.Range(-shakeAmountY, shakeAmountY);

            float lerpFactor = 1 - elapsedTime / duration;
            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f) * slowdownFactor * lerpFactor;

            cameraTransform.position = originalPosition + shakeOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return the camera to its original position
        cameraTransform.position = originalPosition;
    }
}
