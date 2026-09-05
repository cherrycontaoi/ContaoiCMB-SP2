using UnityEngine;

public class BasketController : MonoBehaviour
{
    public GameObject basketPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemsInBasket = new GameObject[9];

    // private int slotIDCounter;

    void Start()
    {
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

    public void AddToBasket(GameObject itemToAdd, int slotID)
    {
        itemsInBasket[slotID] = itemToAdd;
    }

    public void RemoveFromBasket(int slotID)
    {
        itemsInBasket[slotID] = null;
    }
}
