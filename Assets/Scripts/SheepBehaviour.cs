using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


//MESSAGE FOR MK, THE ANIMATION SCRIPT SHOULD BE ADDED IN THE ISRECOVERING METHOD

public class SheepBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    [SerializeField] float minIdleTime = 1f;
    [SerializeField] float maxIdleTime = 3f;
    [SerializeField] float flungRecoveryTime = 3.5f;
    
    GameObject Perimeter;
    GameObject grabbedBy;
    Transform pos1;
    Transform pos2;
    Rigidbody2D sheep;
    SheepEscapeManager escapeManager;

    Vector2 randomPosition;
    Vector2 sheepAngle;
    Vector2 escapeDirection;
    float maxRad;
    float minRad;
    float sheepPositionY;
    int rockFlyingLayer;
    bool isStraight;
    bool isCaught = false;
    bool isRecovering = false;
    bool isKidnapped = false;
    bool isEscaped = false;

    public static int numOfEscaped = 0;

    void Awake()
    {
        //changes angles to rads for ease of use
        maxRad = Mathf.Deg2Rad * maxAngle;
        minRad = Mathf.Deg2Rad * minAngle;
        sheep = GetComponent<Rigidbody2D>();
        Perimeter = GameObject.FindGameObjectWithTag("Perimeter");
        pos1 = Perimeter.transform.GetChild(0);
        pos2 = Perimeter.transform.GetChild(1);
    }

    void Start()
    {
        rockFlyingLayer = LayerMask.NameToLayer("RockFlying");
        escapeManager = SheepEscapeManager.Instance;
        StartCoroutine(ChooseRandomPosition());
    }

    void Update()
    {
        if (!isEscaped && !isRecovering)
        {
            sheep.MovePosition(Vector2.MoveTowards(sheep.position, randomPosition, speed * 2 * Time.deltaTime));
        }
        else if (isEscaped && !isRecovering)
        {
            sheep.linearVelocity = escapeDirection * speed;
        }

        if (numOfEscaped < 0)
        {
            numOfEscaped = 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == rockFlyingLayer)
        {
            float impactForce = collision.relativeVelocity.magnitude;

            if (impactForce > 5f)
            {
                if(GetGrabbedBy() != null)
                {
                    GetGrabbedBy().GetComponent<BanditBehaviour>().DropSheep();
                }
                setIsRecovering(true);
                StartRecovery();
            }
        }
    }

    //calculates and chooses a suitable angle for the sheep to escape through
    public void EscapePen()
    {
        if (isEscaped == false)
        {
            setIsEscaped(true);
        }

        isStraight = Random.Range(0, 2) == 1; //gives a 50% chance for the sheep to move straight

        float randomAngle = Random.Range(minRad, maxRad); //chooses angle of the sheep (incase isStraight is false)
        
        sheepPositionY = sheep.transform.position.y;

        //if the sheep is in the upper half of the screen, makes it move down (if not straight)
        if (!isStraight && sheepPositionY > 1)
        {
            sheepAngle = new Vector2(-Mathf.Cos(randomAngle), -Mathf.Sin(randomAngle));
        }
        //if the sheep is in the lower half of the screen, makes it move up (if not straight)
        else if (!isStraight && sheepPositionY < -1)
        {
            sheepAngle = new Vector2(-Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        }
        //if the sheep is around the middle of the screen, it can either move up or down (if not straight)
        else if (!isStraight)
        {
            int posOrNeg = (Random.value > 0.5f) ? -1:1;
            sheepAngle = new Vector2(-Mathf.Cos(randomAngle), posOrNeg * Mathf.Sin(randomAngle));
        }
        //if the sheep is supposed to be straight, sets its angle accordingly
        else
        {
            sheepAngle = new Vector2 (-1, 0);
        }
        
        escapeDirection = sheepAngle;
        escapeManager.removeSheepFromList(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Perimeter"))
        {
            if (isEscaped)
            {
                setIsEscaped(false);
            }
            if (escapeManager != null)
            {
                escapeManager.addSheepToList(gameObject);
            }
            if  (sheep != null)
            {
                sheep.linearVelocity = Vector2.zero;
            }
        }
    }

    IEnumerator ChooseRandomPosition()
    {
        while (true)
        {
            float posX = Random.Range(pos1.position.x, pos2.position.x);
            float posY = Random.Range(pos1.position.y, pos2.position.y);
            randomPosition = new(posX, posY);
            float timeIdled = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitUntil(() => !isEscaped);
            yield return new WaitForSeconds(timeIdled);
        }
    }

    public void killSheep()
    {
        SheepBehaviour behaviour = gameObject.GetComponent<SheepBehaviour>();
        if (escapeManager.sheepList.Contains(gameObject))
        {
            escapeManager.removeSheepFromList(gameObject);
            Destroy(gameObject);
        }
        else if (behaviour.getIsEscape() == true)
        {
            SheepBehaviour.numOfEscaped -=1;
            Destroy(gameObject);
        }
        else if (behaviour.getIsKidnapped() == true)
        {
            Destroy(gameObject);
        }
    }    

    public void setIsEscaped(bool isEscaped)
    {
        if (isEscaped == true)
        {
            this.isEscaped = isEscaped;
            numOfEscaped += 1;
            Debug.Log("num of sheep escapes: " + numOfEscaped);
        }
        else
        {
            this.isEscaped = isEscaped;
            numOfEscaped -= 1;
        }
    }

    public bool getIsEscape()
    {
        return this.isEscaped;
    }

    public void setIsRecovering(bool isRecovering)
    {
        if (!this.isRecovering)
        {
            //play the fall over animation
        }
        this.isRecovering = isRecovering;
    }

    public bool getIsRecovering()
    {
        return isRecovering;
    }

    public void setIsKidnapped(bool isKidnapped)
    {
        this.isKidnapped = isKidnapped;
    }

    public bool getIsKidnapped()
    {
        return isKidnapped;
    }

    public void SetGrabbedBy(GameObject bandit)
    {
        grabbedBy = bandit;
    }

    public GameObject GetGrabbedBy()
    {
        if (grabbedBy != null)
        {
            return grabbedBy;
        }
        else
        {
            return null;
        }
    }

    public IEnumerator StartRecovery()
    {
        yield return new WaitForSeconds(flungRecoveryTime);
        isRecovering = false;
    }
}
