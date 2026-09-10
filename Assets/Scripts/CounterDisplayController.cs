using System.Collections;
using UnityEngine;

public class CounterDisplayController : MonoBehaviour
{
    [SerializeField] private GameObject plasticBag;
    [SerializeField] private Sprite[] plasticBagSprites = new Sprite[3];
    [SerializeField] private GameObject givePanel;
    // [SerializeField] private BasketController basketController;
    private bool alreadyInBag;
    private int noOfOrders;
    private SpriteRenderer bagImage;

    private void Start()
    {
        bagImage = plasticBag.GetComponent<SpriteRenderer>();
    }
    
    public void DisplayPlasticBag(int noOfOrders)
    {        
        if (noOfOrders <= 3)
        {
            bagImage.sprite = plasticBagSprites[0];
        }
        else if (noOfOrders <= 6 && noOfOrders > 3)
        {
            bagImage.sprite = plasticBagSprites[1];
        }
        else
        {
            bagImage.sprite = plasticBagSprites[2];
        }

        alreadyInBag = true;
        plasticBag.SetActive(true);
        givePanel.SetActive(false);

        Debug.Log("Put out bag!");

        
        // gameObject.GetComponentInParent<BasketController>().ResetBasket();
        // StartCoroutine(WaitAndClearCoroutine());
    }
    
    public bool IsBagged()
    {
        return alreadyInBag;
    }
    
    public IEnumerator WaitAndClearCoroutine()
    {
        yield return new WaitForSeconds(2f);

        ClearCounter();
        Debug.Log("Ok done clearing everything!");
    }

    private void ClearCounter()
    {
        alreadyInBag = false;
        plasticBag.SetActive(false);

        Debug.Log("Clear the counter!");
    }
}
