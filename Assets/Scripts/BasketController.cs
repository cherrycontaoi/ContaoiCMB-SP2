using Unity.VisualScripting;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public GameObject basketPanel;
    public GameObject givePanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemsInBasket = new GameObject[9];
    public int noOfItemsInBasket;
    
    private CounterDisplayController counterDisplayController;
    // private int slotIDCounter;

    void Start()
    {
        counterDisplayController = GetComponent<CounterDisplayController>();

        for(int i=0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, basketPanel.transform).GetComponent<Slot>();
            slot.slotID = i;
            if (i < itemsInBasket.Length && itemsInBasket[i] !=null)
            {
                GameObject item = Instantiate(itemsInBasket[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // item is centered in the slot
                slot.currentItem = item;
            }
        }
    }

    void Update()
    {
        if (noOfItemsInBasket >= 1)
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
        itemsInBasket[slotID] = itemToMove;
    }

    public void EmptySlot(int slotID)
    {
        itemsInBasket[slotID] = null;
    }

    public void AddToBasket(GameObject itemToAdd, int slotID)
    {
        itemsInBasket[slotID] = itemToAdd;
        noOfItemsInBasket++;
    }

    public void RemoveFromBasket(int slotID)
    {
        itemsInBasket[slotID] = null;
        noOfItemsInBasket--;
    }
    
    public void ResetBasket()
    {
        Slot[] slots = basketPanel.GetComponentsInChildren<Slot>();
        
        for(int i=0; i<slotCount; i++)
        {
            itemsInBasket[i] = null;
            slots[i].currentItem = null;
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
            
        }
        noOfItemsInBasket = 0;
    }

    public void DecrementNoOfItems()
    {
        noOfItemsInBasket--;
    }
}
