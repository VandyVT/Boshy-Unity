using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public GameObject playerObject;

    [SerializeField] float moveAmountX = 13.14f;
    [SerializeField] float moveAmountY = 10f;
    [SerializeField] float tolerance = 1f; // this is now in WORLD UNITS

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
            CheckPlayerBoundsAndMove();
        }
    }

    void CheckPlayerBoundsAndMove()
    {
        Vector3 playerPos = playerObject.transform.position;
        Vector3 camPos = transform.position;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float leftBound = camPos.x - camWidth - tolerance;
        float rightBound = camPos.x + camWidth + tolerance;
        float bottomBound = camPos.y - camHeight - tolerance;
        float topBound = camPos.y + camHeight + tolerance;

        bool moved = false;

        if (playerPos.x < leftBound)
        {
            camPos.x -= moveAmountX;
            moved = true;
        }
        else if (playerPos.x > rightBound)
        {
            camPos.x += moveAmountX;
            moved = true;
        }

        if (playerPos.y < bottomBound)
        {
            camPos.y -= moveAmountY;
            moved = true;
        }
        else if (playerPos.y > topBound)
        {
            camPos.y += moveAmountY;
            moved = true;
        }

        if (moved)
            transform.position = camPos;
    }

    public void Shake(float shakeAmountX, float shakeAmountY, float duration)
    {
        if (playerObject != null)
        {
            CheckPlayerBoundsAndMove();
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

        cameraTransform.position = originalPosition;
    }
}
