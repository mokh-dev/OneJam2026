using UnityEngine;

public class RockScript : MonoBehaviour
{
    Collider2D rockCollider;
    Rigidbody2D rockRb;
    Collider2D sheepCollider;
    Collider2D banditCollider;

    bool isColliding = true;
    int sheepLayer;
    int banditLayer;
    int rockLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rockCollider = GetComponent<Collider2D>();
        rockRb = GetComponent<Rigidbody2D>();
        sheepLayer = LayerMask.NameToLayer("Sheep");
        banditLayer = LayerMask.NameToLayer("Bandits");
        rockLayer = LayerMask.NameToLayer("Rock");

    }

    // Update is called once per frame
    void Update()
    {
        if (rockRb.linearVelocity == Vector2.zero)
        {
            DissableCollisions();
        }
        else
        {
            EnableCollisions();
        }
    }

    void DissableCollisions()
    {
        if (isColliding)
        {
            Physics2D.IgnoreLayerCollision(rockLayer, sheepLayer, true);
            Physics2D.IgnoreLayerCollision(rockLayer, banditLayer, true);
            isColliding = false;
        }
    }

    void EnableCollisions()
    {
        if (!isColliding)
        {
            Physics2D.IgnoreLayerCollision(rockLayer, sheepLayer, false);
            Physics2D.IgnoreLayerCollision(rockLayer, banditLayer, false); 
            isColliding = true;
        }
    }
}
