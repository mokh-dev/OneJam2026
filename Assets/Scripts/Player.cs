using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private GameObject _lassoPre; 
    [SerializeField] private float _lassoThrowForce; 
    [SerializeField] private float _lassoPullForce; 

    private bool hasLasso = true;
    private GameObject activeLasso;




    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        //if (hasLasso == false && )
    }

    private void ThrowLasso(Vector2 direction, float throwForce)
    {
        hasLasso = false;

        float rotationAngle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward); 

        activeLasso = Instantiate(_lassoPre, (Vector2)transform.position, rotation);
        activeLasso.GetComponent<Rigidbody2D>().AddForce(direction * throwForce, ForceMode2D.Impulse);
    }

    private void RetractLasso()
    {
        
    }

    public void OnGroundMouseDown(PointerEventData eventData)
    {
        Vector2 pointingVector = (GetMousePosition() - (Vector2)transform.position).normalized;  

        if (hasLasso == true) ThrowLasso(pointingVector, _lassoThrowForce);     
    }
    public void OnGroundMouseUp(PointerEventData eventData)
    {
        if (hasLasso == true) return;

    }


    private Vector2 GetMousePosition()
    {
        return cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
