using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "SavePoint")
        {
            collision.GetComponent<SavePoint>().Save();
        }

        DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}