using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public int numOfItemsInBasket;

    [SerializeField] private GameObject basketPanel;
    [SerializeField] private GameObject givePanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount;
    [SerializeField] public GameObject[] basketSlots = new GameObject[9];
    
    
    // private CounterDisplayController counterDisplayController;
    // private int slotIDCounter;

    void Start()
    {
        // counterDisplayController = GetComponent<CounterDisplayController>();

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
    }

    public void RemoveFromBasket(int slotID)
    {
        basketSlots[slotID] = null;
        numOfItemsInBasket--;
    }
    
    public void ResetBasket()
    {
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
    }

    public void DecrementNoOfItems()
    {
        numOfItemsInBasket--;
    }

    public GameObject[] GetActualItemsInBasket()
    {
        GameObject[] actualItems = new GameObject[numOfItemsInBasket];
        int index = 0;

        foreach(GameObject basketSlot in basketSlots)
        {
            if (basketSlot != null)
            {
                actualItems[index] = basketSlot;
                index++;
            }
        }

        return actualItems;
    }
}
