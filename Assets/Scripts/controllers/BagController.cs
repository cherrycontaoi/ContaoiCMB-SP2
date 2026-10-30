using System.Collections.Generic;
using UnityEngine;

public class BagController : MonoBehaviour
{
    public int numOfItemsInBag;

    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject givePanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount;
    [SerializeField] public GameObject[] bagSlots = new GameObject[9];

    [SerializeField] public List<GameObject> bagQueue = new List<GameObject>();  // based on the order items were put in the bag
    
    public GameObject holdObject;
    // private CounterDisplayController counterDisplayController;
    // private int slotIDCounter;

    void Start()
    {
        bagPanel.SetActive(false);       // don't show yet pag di pa nakapili ng bag 

        // initialize bagQueue slots
        for(int i=0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, bagPanel.transform).GetComponent<Slot>();
            slot.slotID = i;
            if (i < bagSlots.Length && bagSlots[i] !=null)
            {
                GameObject item = Instantiate(bagSlots[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // item is centered in the slot
                slot.currentItem = item;
            }
        }
    }

    void Update()
    {
        if (numOfItemsInBag >= 1)
        {
            givePanel.SetActive(true);
        }
        else
        {
            givePanel.SetActive(false);
        }
    }

    public Slot GetSlotWithID(int ID)
    {
       Slot freeSlot = null;

       foreach (Slot slot in bagPanel.GetComponentsInChildren<Slot>())
        {
            if (slot.slotID == ID)
            {
                freeSlot = slot;
                break;
            }
        }

        return freeSlot;
    }

    public void PlayerHasChosenBag()
    {
        bagPanel.SetActive(true);
    }

    public int GetFirstEmptySlotID()
    {
        for(int i=0; i<bagSlots.Length; i++)
        {
            if (bagSlots[i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetNumOfOccupiedSlots()
    {
        int occupiedSlots = 0;
        
        foreach(GameObject item in bagSlots)
        {
            if (item != null)
            {
                occupiedSlots++;
            }
        }

        return occupiedSlots;
    }

    public void SwapInBag(int slot1, int slot2)
    {
        GameObject temp = bagSlots[slot1];

        bagSlots[slot1] = bagSlots[slot2];
        bagSlots[slot2] = temp;
    }
    public void MoveInBag(GameObject itemToMove, int slotID)
    {
        bagSlots[slotID] = itemToMove;
    }

    public void EmptySlot(int slotID)
    {
        bagSlots[slotID] = null;
    }

    public void AddToBag(GameObject itemToAdd, int slotID)
    {
        bagSlots[slotID] = itemToAdd;
        numOfItemsInBag++;

        // enqueueueueue
        bagQueue.Add(itemToAdd);
    }

    public void RemoveFromBag(int slotID)
    {
        bagSlots[slotID] = null;
        numOfItemsInBag--;

        // dequeue
        foreach(GameObject item in bagQueue)
        {
            if (item.GetComponent<Item>().GetSlotID() == slotID)
            {
                bagQueue.Remove(item);
                break;
            }
        }
    }
    
    public void ResetBag()
    {
        bagPanel.SetActive(false);
        Slot[] slots = bagPanel.GetComponentsInChildren<Slot>();
        
        for(int i=0; i<slotCount; i++)
        {
            bagSlots[i] = null;
            slots[i].currentItem = null;
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
            
        }
        numOfItemsInBag = 0;
        bagQueue.Clear();
    }

    public void DecrementNoOfItems()
    {
        numOfItemsInBag--;
    }

    public List<GameObject> GetActualItemsInBag()
    {
        bagQueue.RemoveAll(item => item == null);
        return bagQueue;
    }
    
    // returns the bag without removing the empty slots
    public GameObject[] GetFullBag()
    {
        return bagSlots;
    }
}
