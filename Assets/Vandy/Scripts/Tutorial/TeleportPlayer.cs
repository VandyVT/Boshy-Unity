using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip teleportClip;

    [SerializeField] Transform teleportPos;

    [Header("Bools")]
    [SerializeField] bool isWarning;
    [SerializeField] bool camShake;
    [SerializeField] bool setPos = true;
    [SerializeField] bool isSceneWarp; bool hasActivated;
    bool sceneWarpActivated = false;

    [Header("Camera Shake")]
    [SerializeField] float shakeAmountX, shakeAmountY, duration;

    [Header("Teleport Device Sprites")]
    [Tooltip("The gameObject references to the sprites represented by an element! 0 = Off | 1 = On | 2 = Broken")]
    [SerializeField] GameObject[] tpSprites;

    [SerializeField] UnityEvent runEvent;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Teleport();
        }
    }

    void Teleport()
    {
        if (isWarning)
        {
            PlayerUiManager.instance.WarningFlash();
        }

        if (camShake)
        {
            CameraManager.instance.Shake(shakeAmountX, shakeAmountY, duration);
        }

        if (isSceneWarp)
        {
            if (hasActivated) return;

            GameManager.Instance.isWarping = true;
            PlayerUiManager.instance.Teleporting();

            audioSource.PlayOneShot(teleportClip);

            sceneWarpActivated = true;
            hasActivated = true;

            tpSprites[0].SetActive(false);
            tpSprites[1].SetActive(true);

            runEvent.Invoke();
        }

        if (setPos)
        {
            PlayerCharacter.instance.transform.position = teleportPos.position;
            audioSource.PlayOneShot(teleportClip);
        }
    }

    void FixedUpdate()
    {
        if (sceneWarpActivated)
        {
            PlayerCharacter.instance.transform.position = this.transform.position;
        }
    }
}