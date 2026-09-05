using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isInBasket;
    private bool isBeingDragged;
    private float holdDownStartTime;
    private float holdDownTime;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (isInBasket && isBeingDragged==false)
        {
            Debug.Log("clicked!");
            holdDownStartTime = Time.time;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isInBasket && isBeingDragged==false)
        {
            Debug.Log("unclicked!");
            holdDownTime = Time.time - holdDownStartTime;
            Debug.Log(holdDownTime);
        }

        if (holdDownTime >= 3)
        {
            Destroy(gameObject);        // remove from basket
        }
    }

    


    public void PlacedInBasket()
    {
        isInBasket = true;
        Debug.Log("Item is dropped in the basket");
    }

    public void BeingDragged()
    {
        isBeingDragged = true;
    }
    public void NotBeingDragged()
    {
        isBeingDragged = false;
    }


}
