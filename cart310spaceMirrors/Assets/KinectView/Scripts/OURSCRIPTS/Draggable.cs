using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject pieceSpot;
    public bool locked = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!locked) {
            Debug.Log("Started Dragging");
            transform.SetAsLastSibling();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!locked)
        {
            Debug.Log("Now Dragging");
            this.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!locked)
        {
            float dist = Vector3.Distance(this.transform.position, pieceSpot.transform.position);
            if (dist < 20)
            {
                Debug.Log("Dragging has stopped.");
                this.transform.position = pieceSpot.transform.position;
                locked = true;
            }
        }

    }
}
