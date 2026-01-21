using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NUnit.Framework;

public class Player : MonoBehaviour
{
    [HideInInspector] public bool IsHoldingMouseDown;
    private Camera cam;

    [SerializeField] private GameObject _lassoPre; 
    [SerializeField] private float _playerMoveSpeed; 
    [SerializeField] private float _lassoThrowForce; 
    [SerializeField] private float _lassoRetractingForce; 
    [SerializeField] private float _lassoSpinSpeed; 
    [SerializeField] private float _lassoFlingForce; 

    private bool hasLasso = true;
    private bool isRetractingLasso;
    private bool lassoFull;

    private GameObject grabbedObj;
    private GameObject activeLasso;
    private Rigidbody2D activeLassoRB;
    private Rigidbody2D playerRB;
    private Vector2 moveDir;





    void Start()
    {
        cam = Camera.main;
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        PlayerMovement();

        IsHoldingMouseDown = Input.GetButton("Fire1");
        
        if (isRetractingLasso == true)
        {
            RetractLasso();
            if (LassoInPlayerBounds() == true) HolsterLasso();
        }

        if (lassoFull == true)
        {
            SpinLasso();
        }
    }

    public void OnGroundMouseDown(PointerEventData eventData)
    {
        Vector2 pointingVector = (GetMousePosition() - (Vector2)transform.position).normalized;  

        if (hasLasso == true) ThrowLasso(pointingVector, _lassoThrowForce);     
    }
    public void OnGroundMouseUp(PointerEventData eventData)
    {
        if (hasLasso == false) StartRetractingLasso();
    }
    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(horizontalInput, verticalInput).normalized;


        playerRB.linearVelocity = moveDir * _playerMoveSpeed;

    }

    private void SpinLasso()
    {
        playerRB.MoveRotation(playerRB.rotation + _lassoSpinSpeed * Time.fixedDeltaTime);
    }

    public void LassoGrabbed(Lassoable lassoable)
    {
        Vector2 lassoDirection = ((Vector2)activeLasso.transform.position - (Vector2)transform.position).normalized;

        float playerRotationAngle = (Mathf.Atan2(lassoDirection.y, lassoDirection.x) * Mathf.Rad2Deg) - 90;
        playerRB.SetRotation(playerRotationAngle);

        activeLassoRB.linearVelocity = Vector2.zero;

        grabbedObj = lassoable.gameObject;
        lassoFull = true;
    }


    private void ThrowLasso(Vector2 direction, float throwForce)
    {
        hasLasso = false;

        float lassoRotationAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
        Quaternion lassoRotation = Quaternion.AngleAxis(lassoRotationAngle, Vector3.forward); 

        activeLasso = Instantiate(_lassoPre, (Vector2)transform.position, lassoRotation);
        activeLassoRB = activeLasso.GetComponent<Rigidbody2D>();
        activeLassoRB.AddForce(direction * throwForce, ForceMode2D.Impulse);

        activeLasso.GetComponent<Lasso>().PlayerScript = this;
    }

    private void HolsterLasso()
    {
        isRetractingLasso = false;

        Destroy(activeLasso);
        activeLasso = null;

        hasLasso = true;
    }

    private void StartRetractingLasso()
    {
        isRetractingLasso = true;


        if (lassoFull)
        {
            lassoFull = false;

            activeLasso.GetComponent<Lasso>().PlayerLetGo(_lassoFlingForce);
        }
    }

    private void RetractLasso()
    {
        Vector2 retractingDir = ((Vector2)transform.position - (Vector2)activeLasso.transform.position).normalized;
        activeLassoRB.AddForce(retractingDir * _lassoRetractingForce);
    }

    private bool LassoInPlayerBounds()
    {
        Vector2 lassoPos = activeLasso.transform.position;

        float boundDistance = 0.5f;

        if (Vector2.Distance(lassoPos, (Vector2)transform.position) > boundDistance) return false;

        return true;
    }

    private Vector2 GetMousePosition()
    {
        return cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
