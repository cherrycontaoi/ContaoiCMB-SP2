using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] public GameObject basketPanel;
    private BasketController basketController;
    private GameObject newItem;
    private CanvasGroup canvasGroup;
    private ItemDragHandler itemDragHandler;
    private Item itemComponent;
    private bool isDragging;

    private void Start()
    {
        basketController = FindAnyObjectByType<BasketController>();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PauseController.IsGamePaused) return;
        // no plastic bag yet
        if (!CounterDisplayController.Instance.CanUserStartDragging())
        {
            isDragging = false;
            Debug.Log("Choose a bag first!");
            return;
        }

        // basket is full
        if (basketController.GetNumOfOccupiedSlots() == 9)
        {
            isDragging = false;
            Debug.Log("The basket is full, put back an item first before adding again.");
            return;
        }
        
        isDragging = true;

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
        if (PauseController.IsGamePaused) return;
        if (!isDragging) return;
        newItem.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PauseController.IsGamePaused) return;
        if (!isDragging) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        itemDragHandler.OnEndDrag(eventData);

        // clear
        newItem = null;
        canvasGroup = null;
        itemDragHandler = null;

        isDragging = false;
        
    }
    

}
