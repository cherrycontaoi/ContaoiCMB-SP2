using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject gameController;
    [SerializeField] private List<GameObject> matchingObjects;   // should be neighbor
    [SerializeField] private ItemType itemType;
    private BasketController basketController;
    private bool isInBasket;
    private bool isBeingDragged;
    private float holdDownStartTime;
    private float holdDownTime;
    [SerializeField] private int slotID;
    [SerializeField] private bool isGlassBottle;
    

    public bool isMatched;
    

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

    public void Unmatch()
    {
        isMatched = false;
    }

    public void Match()
    {
        isMatched = true;
    }

    public int GetSlotID()
    {
        return slotID;
    }

    public void SetSlotID(int ID)
    {
        slotID = ID;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    public bool IsItGlassBottle()
    {
        return isGlassBottle;
    }

    public void AddMatchingObject(GameObject matchingObject)
    {
        matchingObjects.Add(matchingObject);
    }

    public List<string> GetMatchingItemNames()
    {
        List<string> matchingItemNames = new List<string>();

        foreach (GameObject item in matchingObjects)
        {
            matchingItemNames.Add(item.name);
        }

        return matchingItemNames;
    }

    public void ResetMatchingObjects()
    {
        matchingObjects.Clear();
    }


}

public enum ItemType
{
    PartOfPair,        // items with adjacency rules
    Constricted,      // has individual rules
    Free          // no constraint at all
}
