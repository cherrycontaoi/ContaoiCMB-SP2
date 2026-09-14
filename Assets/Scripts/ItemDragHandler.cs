using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent; // slot
    private CanvasGroup canvasGroup;
    private BasketController basketController;
    private CounterDisplayController counterDisplayController;
    private bool freshItem;     // dragged from display
    private bool isDragging;
    private Item itemComponent;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        basketController = FindAnyObjectByType<BasketController>();
        counterDisplayController = FindAnyObjectByType<CounterDisplayController>();
        itemComponent = gameObject.GetComponent<Item>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!counterDisplayController.CanUserStartDragging())
        {
            isDragging = false;
            return;
        }

        isDragging = true;

        itemComponent.BeingDragged();
        originalParent = transform.parent;  // save the original parent
        freshItem = false;

        transform.SetParent(transform.root);    // ensure above any other canvas

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;   // semi-transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isDragging) return;

        transform.position = eventData.position;    // follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!isDragging) return;

        canvasGroup.blocksRaycasts = true;  //enable raycasts
        canvasGroup.alpha = 1f;

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();   // slot to drop item to
        if (dropSlot == null)
        {
            GameObject item = eventData.pointerEnter;
            if (item != null)
            {
                dropSlot = item.GetComponentInParent<Slot>();
            }
        }
        // item is fresh from display (no parent slot)
        if (freshItem)
        {

            if (dropSlot != null)
            {
                if (dropSlot.currentItem != null)
                {
                    Destroy(dropSlot.currentItem);      // replace item
                    basketController.DecrementNoOfItems();
                }

                transform.SetParent(dropSlot.transform);
                
                dropSlot.currentItem = gameObject;
                

                basketController.AddToBasket(gameObject, dropSlot.slotID);
                
                itemComponent.PlacedInBasket();
                
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,0.5f);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,0.5f);
            }
            else
            {
                Destroy(gameObject);        // not dropped in the basket
            }
        }


        // item is just being moved 
        else
        {
            Slot originalSlot = originalParent.GetComponent<Slot>();

            if(dropSlot != null)    // if slot is not empty
            {
                if(dropSlot.currentItem != null)
                {
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    basketController.MoveInBasket(gameObject, dropSlot.slotID);
                }
                else
                {
                    originalSlot.currentItem = null;
                    
                    basketController.EmptySlot(originalSlot.slotID);    // remove from items
                }

                // move item to drop slot
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;

                basketController.MoveInBasket(gameObject, dropSlot.slotID);
            }
            else
            {
                // no slot under drop point
                transform.SetParent(originalParent);    // snap back to orig parent
            }
            itemComponent.PlacedInBasket();
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        
        itemComponent.NotBeingDragged();
        isDragging = false;

    }


    public void ItemIsFresh()
    {
        freshItem = true;
        originalParent = null;
    }


}
