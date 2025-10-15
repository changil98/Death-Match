using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var draggingObj = eventData.pointerDrag.GetComponent<DragHandler>();
        draggingObj?.OnDropped(gameObject.transform);
    }
}