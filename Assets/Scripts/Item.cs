using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField] public string displayName;    // name in dialogue
    [SerializeField] public string supplementaryNameInfo;
    [SerializeField] private int slotID;
    [SerializeField] private bool isGlassBottle;
    
    public GameObject holdObject;
    public Image holdIndicator;
    public bool isHolding = false;
    private float holdDuration = 1f;

    public bool isMatched;
    

    private void Start()
    {
        gameController = GameObject.Find("GameController");
        basketController = gameController.GetComponent<BasketController>();
        
        holdObject = basketController.holdObject;
        holdIndicator = holdObject.GetComponent<Image>();
        holdObject.SetActive(false);
        
    }

    private void Update()
    {
        if (isHolding)
        {
            holdIndicator.transform.position = gameObject.transform.position;
            holdIndicator.fillAmount = (Time.time - holdDownStartTime) / holdDuration;  // 2 secs
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (PauseController.IsGamePaused) return;
        if (isInBasket && isBeingDragged==false)
        {
            Debug.Log("clicked!");
            holdDownStartTime = Time.time;
            isHolding = true;

            holdIndicator.fillAmount = 0;
            holdIndicator.gameObject.SetActive(true);
            holdIndicator.transform.position = gameObject.transform.position;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (PauseController.IsGamePaused) return;

        if (isInBasket && isBeingDragged==false && isHolding)
        {
            Debug.Log("unclicked!");
            holdDownTime = Time.time - holdDownStartTime;
            Debug.Log(holdDownTime);

            isHolding = false;
            holdIndicator.fillAmount = 0;
            holdIndicator.gameObject.SetActive(false);

            if (holdDownTime >= holdDuration)
            {
                Destroy(gameObject);        // remove from basket
                basketController.RemoveFromBasket(slotID);
            }
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
