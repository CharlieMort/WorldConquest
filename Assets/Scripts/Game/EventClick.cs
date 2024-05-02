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
        print("Click" + transform.parent.name);
        GameMasterScript.Instance.SelectionHandler.SelectCountry(transform.parent.gameObject); // will error if not a country
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       //print("Release");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Hover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //print("Stop Hover");
    }
}
