using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject gameController;
    private BasketController basketController;
    private bool isInBasket;
    private bool isBeingDragged;
    private float holdDownStartTime;
    private float holdDownTime;
    private int slotID;

    public void Start()
    {
        gameController = GameObject.Find("GameController");
        basketController = gameController.GetComponent<BasketController>();

        
    }


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
            basketController.RemoveFromBasket(slotID);
        }
    }

    public void PlacedInBasket()
    {
        GetComponent<Image>().SetNativeSize();
        isInBasket = true;
        slotID = gameObject.GetComponentInParent<Slot>().slotID;
        Debug.Log(gameObject.name + " is dropped in the basket");
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
