using UnityEngine;

public class BasketController : MonoBehaviour
{
    public GameObject basketPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    void Start()
    {
        for(int i=0; i<slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, basketPanel.transform).GetComponent<Slot>();

            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // item is centered in the slot
                slot.currentItem = item;
            }
        }
    }

}
