using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] public GameObject basketPanel;
    private GameObject newItem;
    private CanvasGroup canvasGroup;
    private ItemDragHandler itemDragHandler;
    private Item itemComponent;
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        newItem = Instantiate(itemPrefab,basketPanel.transform);
        newItem.transform.position = eventData.position;
        newItem.name = itemPrefab.name;
    

        canvasGroup = newItem.GetComponent<CanvasGroup>();
        itemDragHandler = newItem.GetComponent<ItemDragHandler>();
        itemComponent = newItem.GetComponent<Item>();

        itemComponent.BeingDragged();
        itemDragHandler.ItemIsFresh();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        newItem.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        itemDragHandler.OnEndDrag(eventData);

        // clear
        newItem = null;
        canvasGroup = null;
        itemDragHandler = null;

        

        
    }

    

}
