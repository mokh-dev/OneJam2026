using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using NUnit.Framework;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [HideInInspector] public bool IsHoldingMouseDown;
    private Camera cam;

    [SerializeField] private GameObject _lassoPre; 
    [SerializeField] private GameObject _ropeSectionPre; 
    [SerializeField] private GameObject _ropeAnchor; 
    [SerializeField] private float _ropeSectionCount; 
    [SerializeField] private float _ropeSectionLength; 
    [SerializeField] private float _playerMoveSpeed; 
    [SerializeField] private float _lassoThrowForce; 
    [SerializeField] private float _lassoRetractingForce; 
    [SerializeField] private float _lassoBaseSpinSpeed; 
    [SerializeField] private float _lassoSpinIncreaseCoefficient; 
    [SerializeField] private float _lassoSpinDistanceBias; 
    [SerializeField] private float _lassoBaseFlingForce; 
    [SerializeField] private float _lassoSpinFlingForceBias; 

    private bool holdingLasso = true;
    private bool isRetractingLasso;
    private bool lassoFull;
    private bool lassoIsSpinning;

    private bool ropeSectionJointsActive;

    private int spinIncreaseCount;
    private int spinDirection;
    private int initialMouseQuadrant;
    private int currentMouseQuadrant;
    private int previousMouseQuadrant;
    private int currentSpunMouseQuadrant;
    private int previousSpunMouseQuadrant;

    private GameObject grabbedObj;
    private List<GameObject> spawnedRopeSections = new List<GameObject>();
    private Lasso activeLasso;
    private Rigidbody2D activeLassoRB;
    private Rigidbody2D playerRB;
    private Vector2 moveDirection;
    private float spinSpeed;
    





    void Start()
    {
        cam = Camera.main;
        playerRB = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerMovement();

        IsHoldingMouseDown = Input.GetButton("Fire1");

        if (holdingLasso == false && isRetractingLasso == false) RopeSpawning();

        if (holdingLasso == false && isRetractingLasso == false && ropeSectionJointsActive == false && spawnedRopeSections.Count >= _ropeSectionCount) ActivateRopeJoints();
        
        if (isRetractingLasso == true)
        {
            RetractRope();

            if (ObjInPlayerBounds(activeLasso.gameObject) == true) HolsterLasso();
        }

        if (lassoFull == true) CheckForNewMouseQuadrant();

        if (lassoIsSpinning) SpinLasso();

    }

    private void RetractRope()
    {
        if (spawnedRopeSections.Count <= 0 ) return;
        if (ObjInPlayerBounds(spawnedRopeSections[spawnedRopeSections.Count - 1]) == true) RemoveRopeSectionJointConnnections();
        if (spawnedRopeSections.Count <= 0 ) return;

        Vector2 retractingDir = ((Vector2)transform.position - (Vector2)spawnedRopeSections[spawnedRopeSections.Count - 1].transform.position).normalized;
        spawnedRopeSections[spawnedRopeSections.Count - 1].GetComponent<Rigidbody2D>().AddForce(retractingDir * _lassoRetractingForce);
    }

    private void ActivateRopeJoints()
    {
        ropeSectionJointsActive = true;
        foreach (GameObject ropeSection in spawnedRopeSections)
        {
            ropeSection.GetComponent<HingeJoint2D>().enabled = true;
        }
    }

    private void RemoveRopeSectionJointConnnections()
    {
        GameObject lastRopeSection = spawnedRopeSections[spawnedRopeSections.Count - 1];

        
        spawnedRopeSections.Remove(lastRopeSection);

        Destroy(lastRopeSection);

        if (spawnedRopeSections.Count < 1) return;

        GameObject newLastRopeSection = spawnedRopeSections[spawnedRopeSections.Count - 1];

        newLastRopeSection.GetComponent<HingeJoint2D>().enabled = false;
    }

    public void OnGroundMouseDown(PointerEventData eventData)
    {
        Vector2 pointingVector = (GetMousePosition() - (Vector2)transform.position).normalized;  

        if (holdingLasso == true) ThrowLasso(pointingVector, _lassoThrowForce);     
    }
    public void OnGroundMouseUp(PointerEventData eventData)
    {
        if (lassoIsSpinning == true) FlingObject();
        
        if (holdingLasso == false) StartRetracting();
    }

    private void RopeSpawning()
    {
        if (spawnedRopeSections.Count >= _ropeSectionCount) return;

        Vector2 lassoPos = activeLasso.gameObject.transform.position;
        if (Vector2.Distance(transform.position, lassoPos) >= _ropeSectionLength)
        {
            SpawnRopeSection();
        }
    }

    private void SpawnRopeSection()
    {
        Vector2 lassoDirection = ((Vector2)activeLasso.gameObject.transform.position - (Vector2)transform.position).normalized;

        float ropeSectionAngle = (Mathf.Atan2(lassoDirection.y, lassoDirection.x) * Mathf.Rad2Deg) - 90;
        Quaternion ropeSectionRotation = Quaternion.AngleAxis(ropeSectionAngle, Vector3.forward); 

        GameObject ropeSectionObj = Instantiate(_ropeSectionPre, transform.position, ropeSectionRotation);

        AddRopeSectionJointConnnections(ropeSectionObj);
        
    }

    private void AddRopeSectionJointConnnections(GameObject newRopeSection)
    {
        HingeJoint2D ropeSectionJoint = newRopeSection.GetComponent<HingeJoint2D>();
        ropeSectionJoint.gameObject.name = "RopeSection " + spawnedRopeSections.Count.ToString();

        if (spawnedRopeSections.Count == 0)
        {
            ropeSectionJoint.connectedBody = _ropeAnchor.GetComponent<Rigidbody2D>();
            ropeSectionJoint.anchor = new Vector2(0, 1);
            ropeSectionJoint.connectedAnchor = new Vector2(0, 0);

            activeLasso.GetComponent<HingeJoint2D>().connectedBody = newRopeSection.GetComponent<Rigidbody2D>();
          
        } 
        else
        {
            HingeJoint2D previousJoint = spawnedRopeSections[spawnedRopeSections.Count - 1].GetComponent<HingeJoint2D>();
            previousJoint.connectedBody = ropeSectionJoint.GetComponent<Rigidbody2D>();
            previousJoint.connectedAnchor = new Vector2(0, -1);

            ropeSectionJoint.connectedBody = _ropeAnchor.GetComponent<Rigidbody2D>();
        }

        spawnedRopeSections.Add(newRopeSection);
    }

    private void CheckForNewMouseQuadrant()
    {
        previousMouseQuadrant = currentMouseQuadrant;
        currentMouseQuadrant = GetCurrentMouseQuadrant(GetMousePosition());

        if (initialMouseQuadrant == currentMouseQuadrant && currentSpunMouseQuadrant == 0) return;

        if (currentMouseQuadrant != previousMouseQuadrant)
        {
            MouseSpin();   
        }
    }
    
    private void MouseSpin()
    {
        currentSpunMouseQuadrant = currentMouseQuadrant;
        previousSpunMouseQuadrant = previousMouseQuadrant;

        if (spinDirection == 0) spinDirection = GetSpinDir(previousSpunMouseQuadrant, currentSpunMouseQuadrant); // initial
        if (spinDirection != GetSpinDir(previousSpunMouseQuadrant, currentSpunMouseQuadrant)) return; //switched spin direction

        if (lassoIsSpinning == false) activeLasso.StartedSpinning(); //update lasso once 
        lassoIsSpinning = true;

        float mouseDistanceFromPlayer = Vector2.Distance(GetMousePosition(), (Vector2)transform.position);

        //Debug.Log("Spin Increase: " + Mathf.RoundToInt(mouseDistanceFromPlayer *_lassoSpinDistanceBias).ToString());
        spinIncreaseCount = spinIncreaseCount + Mathf.RoundToInt(mouseDistanceFromPlayer *_lassoSpinDistanceBias * spinDirection);
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;


        playerRB.linearVelocity = moveDirection * _playerMoveSpeed;
    }

    private void SpinLasso()
    {
        spinSpeed = _lassoBaseSpinSpeed + _lassoSpinIncreaseCoefficient * Mathf.Max(0f, Mathf.Log(Mathf.Abs(spinIncreaseCount))) * Mathf.Sign(spinIncreaseCount);

        playerRB.MoveRotation(playerRB.rotation + spinSpeed * Time.fixedDeltaTime);
    }


    public void LassoGrabbed(Lassoable lassoable)
    {
        Vector2 lassoDirection = ((Vector2)activeLasso.gameObject.transform.position - (Vector2)transform.position).normalized;

        float playerRotationAngle = (Mathf.Atan2(lassoDirection.y, lassoDirection.x) * Mathf.Rad2Deg) - 90;
        playerRB.SetRotation(playerRotationAngle);

        activeLassoRB.linearVelocity = Vector2.zero;

        grabbedObj = lassoable.gameObject;

        initialMouseQuadrant = GetCurrentMouseQuadrant(GetMousePosition());
        lassoFull = true;
    }


    private void ThrowLasso(Vector2 direction, float throwForce)
    {
        holdingLasso = false;

        float lassoRotationAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
        Quaternion lassoRotation = Quaternion.AngleAxis(lassoRotationAngle, Vector3.forward); 

        GameObject lassoObj = Instantiate(_lassoPre, (Vector2)transform.position, lassoRotation);

        activeLassoRB = lassoObj.GetComponent<Rigidbody2D>();
        activeLassoRB.AddForce(direction * throwForce, ForceMode2D.Impulse);

        activeLasso = lassoObj.GetComponent<Lasso>();
        activeLasso.PlayerScript = this;
    }

    private void HolsterLasso()
    {
        isRetractingLasso = false;

        Destroy(activeLasso.gameObject);
        activeLasso = null;

        holdingLasso = true;
        ropeSectionJointsActive = false;
    }

    private void FlingObject()
    {
        activeLasso.PlayerFlung((_lassoBaseFlingForce + spinSpeed *_lassoSpinFlingForceBias) * Mathf.Sign(spinDirection));

        lassoIsSpinning = false;
        lassoFull = false;

        spinIncreaseCount = 0;
        currentMouseQuadrant = 0;
        previousMouseQuadrant = 0;
        currentSpunMouseQuadrant = 0;
        previousSpunMouseQuadrant = 0;
        spinDirection = 0;
        spinSpeed = 0;
    }

    private void StartRetracting()
    {
        isRetractingLasso = true;
        
        if (lassoFull)
        {
            lassoFull = false;

            activeLasso.PlayerDropped();
        }
    }



    private int GetCurrentMouseQuadrant(Vector2 mousePos)
    {
        Vector2 playerPos = (Vector2)transform.position;

        if (mousePos.x > playerPos.x && mousePos.y > playerPos.y) return 1;
        if (mousePos.x < playerPos.x && mousePos.y > playerPos.y) return 2;
        if (mousePos.x < playerPos.x && mousePos.y < playerPos.y) return 3;
        if (mousePos.x > playerPos.x && mousePos.y < playerPos.y) return 4;

        return 1; //center
    }


    // 1: positive and Counterclockwise
    // -1: negative and clockwise
    private int GetSpinDir(int previousQuadrant, int currentQuadrant) 
    {
        if (currentQuadrant == previousQuadrant) return 0;

        if (previousQuadrant == 4 && currentQuadrant == 1) return 1; 
        if (previousQuadrant == 1 && currentQuadrant == 4) return -1;

        if (currentQuadrant > previousQuadrant) return 1; //CCW
        if (currentQuadrant < previousQuadrant) return -1; //CW

        return 0;
    }

    private bool ObjInPlayerBounds(GameObject objToCheck)
    {
        float boundDistance = 0.5f;

        if (Vector2.Distance((Vector2)objToCheck.transform.position, (Vector2)transform.position) > boundDistance) return false;

        return true;
    }

    private Vector2 GetMousePosition()
    {
        return cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
