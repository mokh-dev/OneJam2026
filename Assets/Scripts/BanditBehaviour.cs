using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;


//MESSAGE FOR MK, THE ANIMATION SCRIPT SHOULD BE ADDED IN THE DROPSHEEP METHOD

public class BanditBehaviour : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxAngle = 15f;
    [SerializeField] float minAngle = 5f;
    [SerializeField] float timeToDie = 2f;
    [SerializeField] float flungRecoveryTime = 3.5f;
    [SerializeField] float leftOffscreenValue;

    SheepEscapeManager escapeManager;
    SheepBehaviour sheepBehaviour;
    Rigidbody2D bandit;
    Rigidbody2D sheepRb;
    GameObject currentTarget;
    GameObject grabbedSheep;
    Animator banditAnim;

    Vector2 banditRotation;
    float maxRad;
    float minRad;
    float banditPositionY;
    bool isRecovering;
    bool isStraight;

    void Start()
    {
        banditAnim = gameObject.GetComponent<Animator>();

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

    void FixedUpdate()
    {
        if (!isRecovering)
        {
            if(currentTarget != null && grabbedSheep == null)
            {
                if (escapeManager.sheepList.Contains(currentTarget) && !sheepBehaviour.getIsKidnapped() && !sheepBehaviour.getIsEscape())
                {
                    chaseSheep();
                }
                else
                {
                    currentTarget = chooseSheep();
                    if(currentTarget != null)
                    {
                        sheepBehaviour = currentTarget.GetComponent<SheepBehaviour>();
                    }
                }
            }
            else
            {
                SnapSheep();
                runBack();
            }
        }

        if (gameObject.transform.position.x < leftOffscreenValue)
        {
            killBandit();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SheepHoldingBox") && collision.gameObject == currentTarget)
        {
            sheepBehaviour.setIsEscaped(true);
            sheepBehaviour.setIsRecovering(true);
            sheepBehaviour.SetGrabbedBy(gameObject);
            grabbedSheep = currentTarget;
            sheepRb = grabbedSheep.GetComponent<Rigidbody2D>();
            gameObject.GetComponent<Lassoable>().enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;

        }

        if (collision.CompareTag("KillZone"))
        {
            killBandit();
        }
    }

    void runBack()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
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
        if (grabbedSheep != null && sheepRb != null)
        {
            sheepRb.MovePosition(transform.position);
            Debug.Log("grabbed");
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
        Debug.Log("dropped");
        
        banditAnim.SetBool("IsDead", true);
        Invoke("killBandit", timeToDie);
    }

    void killBandit()
    {
        Destroy(gameObject);
    }

    public void setIsRecovering(bool isRecovering)
    {
        this.isRecovering = isRecovering;
    }

    public IEnumerator StartRecovery()
    {
        yield return new WaitForSeconds(flungRecoveryTime);
        isRecovering = false;
    }

    public bool getIsRecovering()
    {
        return isRecovering;
    }
}

