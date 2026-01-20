using UnityEngine;

public class Player : MonoBehaviour
{


    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(Input.mousePosition);
    }

    private Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }
}
