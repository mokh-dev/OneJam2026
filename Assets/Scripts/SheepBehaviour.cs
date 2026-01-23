using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SheepBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    [SerializeField] float minIdleTime = 1f;
    [SerializeField] float maxIdleTime = 3f;
    
    GameObject Perimeter;
    Transform pos1;
    Transform pos2;
    Rigidbody2D sheep;

    Vector2 randomPosition;
    Vector2 sheepRotation;
    float maxRad;
    float minRad;
    float sheepPositionY;
    bool isStraight;
    bool planningEscape = false;
    bool isEscaped = false;

    public static int numOfEscaped;
    public static int numOfplannedEscapes;

    void Start()
    {
        //changes angles to rads for ease of use
        maxRad = Mathf.Deg2Rad * maxAngle;
        minRad = Mathf.Deg2Rad * minAngle;
        sheep = GetComponent<Rigidbody2D>();
        Perimeter = GameObject.FindGameObjectWithTag("Perimeter");
        pos1 = Perimeter.transform.GetChild(0);
        pos2 = Perimeter.transform.GetChild(1);
        StartCoroutine(ChooseRandomPosition());
    }

    void Update()
    {
        if (!isEscaped)
        {
            sheep.MovePosition(Vector2.MoveTowards(sheep.position, randomPosition, speed * Time.deltaTime));
        }
    }

    //calculates and chooses a suitable angle for the sheep to escape through
    public void EscapePen()
    {
        isStraight = Random.Range(0, 2) == 1; //gives a 50% chance for the sheep to move straight

        float randomAngle = Random.Range(minRad, maxRad); //chooses angle of the sheep (incase isStraight is false)
        
        sheepPositionY = sheep.transform.position.y;

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
        
        sheep.linearVelocity = sheepRotation * speed; //makes the sheep move with the chosen angle
        isEscaped = true;
        numOfEscaped += 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == Perimeter.GetComponent<Collider2D>())
        {
            numOfEscaped -= 1;
        }
    }

    IEnumerator ChooseRandomPosition()
    {
        while (!isEscaped)
        {
            float posX = Random.Range(pos1.position.x, pos2.position.x);
            float posY = Random.Range(pos1.position.y, pos2.position.y);
            randomPosition = new(posX, posY);
            float timeIdled = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(timeIdled);
        }
    }

    public void setIsEscaped(bool isEscaped)
    {
        this.isEscaped = isEscaped;
        numOfplannedEscapes -=1;
        flagForEscape(false);
    }

    public bool getIsEscape()
    {
        return this.isEscaped;
    }

    public void flagForEscape(bool planningEscape)
    {
        this.planningEscape = planningEscape;
    }

    public bool getPlanningEscape()
    {
        return this.planningEscape;
    }
}
