using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    public bool canGrantExtraJump = true; // Set this to false initially if you want it disabled.
    public SpriteRenderer sprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCharacter playerController = other.GetComponent<PlayerCharacter>();

            if (playerController != null && canGrantExtraJump)
            {
                // Grant extra jump to the player
                playerController.EnableExtraJump();

                // Disable the sprite renderer temporarily
                sprite.enabled = false;

                // Disable the extra jump bool temporarily
                canGrantExtraJump = false;

                // Set a timer to re-enable the sprite renderer and the bool after a certain duration (e.g., 2 seconds)
                Invoke("ResetItem", 2f);
            }
        }
    }

    // Function to reset the item after a certain duration
    private void ResetItem()
    {
        // Re-enable the sprite renderer
        sprite.enabled = true;

        // Re-enable the extra jump bool
        canGrantExtraJump = true;
    }
}
