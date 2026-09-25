using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public int numOfItemsInBasket;

    [SerializeField] private GameObject basketPanel;
    [SerializeField] private GameObject givePanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount;
    [SerializeField] public GameObject[] basketSlots = new GameObject[9];

    [SerializeField] public List<GameObject> basketQueue = new List<GameObject>();  // based on the order items were put in the bag
    
    // private CounterDisplayController counterDisplayController;
    // private int slotIDCounter;

    void Start()
    {
        basketPanel.SetActive(false);       // don't show yet pag di pa nakapili ng bag 

        // initialize basket slots
        for(int i=0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, basketPanel.transform).GetComponent<Slot>();
            slot.slotID = i;
            if (i < basketSlots.Length && basketSlots[i] !=null)
            {
                GameObject item = Instantiate(basketSlots[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // item is centered in the slot
                slot.currentItem = item;
            }
        }
    }

    void Update()
    {
        if (numOfItemsInBasket >= 1)
        {
            givePanel.SetActive(true);
        }
        else
        {
            givePanel.SetActive(false);
        }
    }

    public void PlayerHasChosenBag()
    {
        basketPanel.SetActive(true);
    }

    public void MoveInBasket(GameObject itemToMove, int slotID)
    {
        basketSlots[slotID] = itemToMove;
    }

    public void EmptySlot(int slotID)
    {
        basketSlots[slotID] = null;
    }

    public void AddToBasket(GameObject itemToAdd, int slotID)
    {
        basketSlots[slotID] = itemToAdd;
        numOfItemsInBasket++;

        // enqueueueueue
        basketQueue.Add(itemToAdd);
    }

    public void RemoveFromBasket(int slotID)
    {
        basketSlots[slotID] = null;
        numOfItemsInBasket--;

        // dequeue
        foreach(GameObject item in basketQueue)
        {
            if (item.GetComponent<Item>().GetSlotID() == slotID)
            {
                basketQueue.Remove(item);
                break;
            }
        }
    }
    
    public void ResetBasket()
    {
        basketPanel.SetActive(false);
        Slot[] slots = basketPanel.GetComponentsInChildren<Slot>();
        
        for(int i=0; i<slotCount; i++)
        {
            basketSlots[i] = null;
            slots[i].currentItem = null;
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
            
        }
        numOfItemsInBasket = 0;
        basketQueue.Clear();
    }

    public void DecrementNoOfItems()
    {
        numOfItemsInBasket--;
    }

    public List<GameObject> GetActualItemsInBasket()
    {
        basketQueue.RemoveAll(item => item == null);
        return basketQueue;
    }
    
    // returns the basket without removing the empty slots
    public GameObject[] GetFullBasket()
    {
        return basketSlots;
    }
}
