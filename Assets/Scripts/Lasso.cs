using UnityEngine;

public class Lasso : MonoBehaviour
{

    [HideInInspector] public Player PlayerScript;

    private Rigidbody2D lassoRB;
    private Rigidbody2D playerRB;
    private Lassoable caughtObj;

    private HingeJoint2D lassoJoint;


    bool isSpinning;


    private void Awake()
    {
        lassoJoint = gameObject.GetComponent<HingeJoint2D>();
        lassoRB = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isSpinning == true)
        {
            lassoRB.SetRotation(playerRB.rotation);
        }
    }

    public void PlayerFlung(float flingForce)
    {
        isSpinning = false;

        caughtObj.LassoLetGo();
        caughtObj.GetComponent<Rigidbody2D>().AddForce(-transform.right * flingForce, ForceMode2D.Impulse);
        caughtObj = null;

        lassoJoint.enabled = false;
        lassoJoint.connectedBody = null;
    }

    public void PlayerDropped()
    {
        caughtObj.LassoLetGo();
        caughtObj = null;

        lassoJoint.enabled = false;
        lassoJoint.connectedBody = null;
    }

    private void LassoCaughtObj()
    {
        caughtObj.LassoConnected(this);

        PlayerScript.LassoGrabbed(caughtObj);
        lassoJoint.enabled = true;

        playerRB = PlayerScript.gameObject.GetComponent<Rigidbody2D>();
        lassoJoint.connectedBody = playerRB;
    } 

    public void StartedSpinning()
    {
        isSpinning = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caughtObj != null) return;
        if (PlayerScript.IsHoldingMouseDown == false) return;
        if (collision.gameObject.TryGetComponent<Lassoable>(out caughtObj) == false) return;
        if (caughtObj.GetComponent<Lassoable>().isActiveAndEnabled == false) return;

        LassoCaughtObj();
    }
}
