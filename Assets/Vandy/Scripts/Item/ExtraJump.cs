using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    public bool canGrantExtraJump = true;
    public SpriteRenderer sprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter playerController = other.GetComponent<PlayerCharacter>();

            if (playerController != null && canGrantExtraJump)
            {
                playerController.EnableExtraJump();
                sprite.enabled = false;
                canGrantExtraJump = false;
                Invoke("ResetItem", 2f);
            }
        }
    }

    private void ResetItem()
    {
        sprite.enabled = true;
        canGrantExtraJump = true;
    }
}
