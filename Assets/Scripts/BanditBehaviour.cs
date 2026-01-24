using Unity.VisualScripting;
using UnityEngine;

public class BanditBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    [SerializeField] float timeToDie = 2f;
    [SerializeField] float flungRecoveryTime = 3.5f;

    SheepEscapeManager escapeManager;
    SheepBehaviour sheepBehaviour;
    Rigidbody2D bandit;
    GameObject currentTarget;
    GameObject grabbedSheep;

    Vector2 banditRotation;
    float maxRad;
    float minRad;
    float banditPositionY;
    bool isRecovering;
    bool isStraight;

    void Start()
    {
        //changes angles to rads for ease of use
        maxRad = Mathf.Deg2Rad * maxAngle;
        minRad = Mathf.Deg2Rad * minAngle;
        escapeManager = SheepEscapeManager.Instance;
        currentTarget = chooseSheep();
        if (currentTarget != null)
        {
            sheepBehaviour = currentTarget.GetComponent<SheepBehaviour>();
        }
        //decides the sheeps direction of movement
        bandit = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isRecovering)
        {
            if(grabbedSheep == null)
            {
                if (currentTarget != null && escapeManager.sheepList.Contains(currentTarget) && !sheepBehaviour.getIsKidnapped())
                {
                    chaseSheep();
                }
                else
                {
                    currentTarget = chooseSheep();
                    sheepBehaviour = currentTarget.GetComponent<SheepBehaviour>();
                }
            }
            else
            {
                SnapSheep();
                runBack();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SheepHoldingBox") && collision.transform.parent.gameObject == currentTarget)
        {
            sheepBehaviour.setIsEscaped(true);
            grabbedSheep = currentTarget;
            gameObject.GetComponent<Lassoable>().enabled = false;
        }
    }

    void runBack()
    {
        bandit.linearVelocity = Vector2.left * speed;
    }

    GameObject chooseSheep()
    {
        if (escapeManager.sheepList.Count != 0)
        {
            int randomIndex = Random.Range(0, escapeManager.sheepList.Count);
            GameObject chosenSheep = escapeManager.sheepList[randomIndex];
            return chosenSheep;
        }
        else
        {
            Debug.Log("couldnt find sheep");
            return null;
        }
    }

    void chaseSheep()
    {
        Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

        bandit.linearVelocity = direction * speed;
    }

    public void SnapSheep()
    {
        if (grabbedSheep != null)
        {
            currentTarget.transform.position = transform.position;
        }
    }

    public void DropSheep()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        bandit.linearVelocity = Vector2.zero;
        isRecovering = true;
        sheepBehaviour.setIsEscaped(false);
        sheepBehaviour.setIsRecovering(true);
        grabbedSheep = null;
        currentTarget = null;
        //code to play kill  animation
        Invoke("killBandit", timeToDie);
    }

    void killBandit()
    {
        Destroy(gameObject);
    }
}

