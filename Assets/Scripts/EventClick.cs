using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private void Awake()
    {
        //
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("Click");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("Release");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Stop Hover");
    }
}
