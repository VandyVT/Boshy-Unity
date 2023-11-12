using UnityEngine;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer bulletSpr;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "SavePoint")
        {
            collision.GetComponent<SavePoint>().Save();
            DestroyBullet();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        { 
            DestroyBullet();
        }
    }

    private void Start()
    {
        this.name = "Bullet";
    }

    void Update()
    {
        if(bulletSpr.isVisible == false)
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}