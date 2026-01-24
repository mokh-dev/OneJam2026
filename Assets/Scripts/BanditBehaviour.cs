using UnityEngine;

public class BanditBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    [SerializeField] float flungRecoveryTime = 3.5f;

    SheepEscapeManager escapeManager;
    SheepBehaviour sheepBehaviour;
    Rigidbody2D bandit;
    GameObject currentTarget;

    Vector2 banditRotation;
    float maxRad;
    float minRad;
    float banditPositionY;
    bool isStraight;

    void Start()
    {
        //changes angles to rads for ease of use
        maxRad = Mathf.Deg2Rad * maxAngle;
        minRad = Mathf.Deg2Rad * minAngle;
        escapeManager = SheepEscapeManager.Instance;
        currentTarget = chooseSheep();
        sheepBehaviour = currentTarget.GetComponent<SheepBehaviour>();

        //decides the sheeps direction of movement
        bandit = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (escapeManager.sheepList.Contains(currentTarget) && !sheepBehaviour.getIsKidnapped())
        {
            chaseSheep();
        }
        else
        {
            currentTarget = chooseSheep();
            sheepBehaviour = currentTarget.GetComponent<SheepBehaviour>();
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

    public void GrabSheep()
    {
        Rigidbody2D sheepRb = currentTarget.GetComponent<Rigidbody2D>();
        currentTarget.transform.position = transform.position;
    }

    public void DropSheep()
    {
        
    }
}

