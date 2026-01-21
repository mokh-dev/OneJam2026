using UnityEngine;

public class SheepBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    
    Rigidbody2D sheep;

    Vector2 sheepRotation;
    float maxRad;
    float minRad;
    float sheepPositionY;
    bool isStraight;

    void Start()
    {
        //changes angles to rads for ease of use
        maxRad = Mathf.Deg2Rad * maxAngle;
        minRad = Mathf.Deg2Rad * minAngle;

        //decides the sheeps direction of movement
        sheep = GetComponent<Rigidbody2D>();
        sheepPositionY = sheep.transform.position.y;
        angleBehaviour();
        
        sheep.linearVelocity = sheepRotation * speed; //makes the sheep move with the chosen angle
    }

    //calculates and chooses a suitable angle for the sheep
    void angleBehaviour()
    {
        isStraight = Random.Range(0, 2) == 1; //gives a 50% chance for the sheep to move straight

        float randomAngle = Random.Range(minRad, maxRad); //chooses angle of the sheep (incase isStraight is false)
        
        //if the sheep is in the upper half of the screen, makes it move down (if not straight)
        if (!isStraight && sheepPositionY > 1)
        {
            sheepRotation = new Vector2(-Mathf.Cos(randomAngle), -Mathf.Sin(randomAngle));
        }

        //if the sheep is in the lower half of the screen, makes it move up (if not straight)
        else if (!isStraight && sheepPositionY < -1)
        {
            sheepRotation = new Vector2(-Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        }

        //if the sheep is around the middle of the screen, it can either move up or down (if not straight)
        else if (!isStraight)
        {
            int posOrNeg = (Random.value > 0.5f) ? -1:1;
            sheepRotation = new Vector2(-Mathf.Cos(randomAngle), posOrNeg * Mathf.Sin(randomAngle));
        }

        //if the sheep is supposed to be straight, sets its angle accordingly
        else
        {
            sheepRotation = new Vector2 (-1, 0);
        }
    }
}
