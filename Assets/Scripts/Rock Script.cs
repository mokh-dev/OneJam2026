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
    int rockStoppedLayer;
    int rockFlyingLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rockCollider = GetComponent<Collider2D>();
        rockRb = GetComponent<Rigidbody2D>();
        sheepLayer = LayerMask.NameToLayer("Sheep");
        banditLayer = LayerMask.NameToLayer("Bandits");
        rockStoppedLayer = LayerMask.NameToLayer("RockStopped");
        rockFlyingLayer = LayerMask.NameToLayer("RockFlying");

    }

    // Update is called once per frame
    void Update()
    {
        if (rockRb.linearVelocity.magnitude < 0.1f)
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
            gameObject.layer = rockStoppedLayer;
            isColliding = false;
        }
    }

    void EnableCollisions()
    {
        if (!isColliding)
        {
            gameObject.layer = rockFlyingLayer;
            Debug.Log("im flying");
            isColliding = true;
        }
    }
}
