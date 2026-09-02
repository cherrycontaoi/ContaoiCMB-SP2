using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent; // slot
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;  // save the original parent
        transform.SetParent(transform.root);    // ensure above any other canvas
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;   // semi-transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;    // follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if(dropSlot != null)    // if slot is not empty
        {
            if(dropSlot.currentItem != null)
            {
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // move item to drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // no slot under drop point
            transform.SetParent(originalParent);    // snap back to orig parent
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

}
