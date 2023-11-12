using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip teleportClip;

    [SerializeField] Transform teleportPos;

    [Header("Bools")]
    [SerializeField] bool isWarning;
    [SerializeField] bool camShake;

    [Header("Camera Shake")]
    [SerializeField] float shakeAmountX, shakeAmountY, duration;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Teleport();
        }
    }

    void Teleport()
    {
        audioSource.PlayOneShot(teleportClip);
        if (isWarning)
        {
            PlayerUiManager.instance.WarningFlash();
        }

        if (camShake)
        {
            CameraManager.instance.Shake(shakeAmountX, shakeAmountY, duration);
        }

        PlayerCharacter.instance.transform.position = teleportPos.position;
    }
}
