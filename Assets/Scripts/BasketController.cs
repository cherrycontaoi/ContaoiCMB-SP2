using UnityEngine;

public class BasketController : MonoBehaviour
{
    public GameObject basketPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemsInBasket;

    void Start()
    {
        for(int i=0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, basketPanel.transform).GetComponent<Slot>();

            if (i < itemsInBasket.Length)
            {
                GameObject item = Instantiate(itemsInBasket[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // item is centered in the slot
                slot.currentItem = item;
            }
        }
    }
}
