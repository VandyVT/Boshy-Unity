using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip teleportClip;

    [SerializeField] Transform teleportPos;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Teleport();
        }
    }

    void Teleport()
    {
        audioSource.PlayOneShot(teleportClip);
        PlayerCharacter.instance.transform.position = teleportPos.position;
    }
}
