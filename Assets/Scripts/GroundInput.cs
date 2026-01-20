using UnityEngine;
using UnityEngine.EventSystems;

public class GroundInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Player player;


    public void OnPointerDown(PointerEventData eventdata)
    {
        player.OnGroundMouseDown(eventdata);
    }
    public void OnPointerUp(PointerEventData eventdata)
    {
        player.OnGroundMouseUp(eventdata);
    }

}
