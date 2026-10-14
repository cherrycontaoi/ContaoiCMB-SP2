using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CounterDisplayController : MonoBehaviour
{
    [SerializeField] private GameObject plasticBag;
    [SerializeField] private Sprite[] plasticBagSprites = new Sprite[3];
    [SerializeField] private GameObject givePanel;
    [SerializeField] private GameObject plasticBagGreen;
    [SerializeField] private GameObject plasticBagYellow;
    [SerializeField] private GameObject plasticBagOrange;
    [SerializeField] private BasketController basketController;
    private bool alreadyInBag;
    private bool userCanStartDragging = false;
    private string colorChosen;
    // private int noOfOrders;
    private SpriteRenderer bagImage;

    private void Start()
    {
        basketController = GetComponent<BasketController>();
        bagImage = plasticBag.GetComponent<SpriteRenderer>();
        userCanStartDragging = false;
    }
    
    public void DisplayPlasticBag(int noOfOrders)
    {        
        alreadyInBag = true;
        plasticBag.SetActive(true);
        givePanel.SetActive(false);
        RemovePBFromPlayerSpace();

        Debug.Log("Bag already on the counter!");
        DialogueController.Instance.ShowDialogueUI(false);      // clear unnecessary lines

        
        // gameObject.GetComponentInParent<BasketController>().ResetBasket();
        // StartCoroutine(WaitAndClearCoroutine());
    }

    public void ChoosePlasticBag(string PBColor)
    {
        Customer customer = FindAnyObjectByType<Customer>();
        // can't choose pb if game is paused or if there are no customers
        if(PauseController.IsGamePaused || customer == null || !customer.willBuy || !customer.currentlyBuying || !customer.GetComponent<WaypointMover>().hasStoppedAtStore || Pencil.Instance.isActive) return;

        Debug.Log("The plastic bag to be used is: " + PBColor);
        if(PBColor == "PBGreen")
        {
            plasticBagYellow.SetActive(false);
            plasticBagOrange.SetActive(false);

            plasticBagGreen.SetActive(true);
            bagImage.sprite = plasticBagSprites[0];
        }

        else if (PBColor == "PBYellow")
        {
            plasticBagGreen.SetActive(false);
            plasticBagOrange.SetActive(false);

            plasticBagYellow.SetActive(true);
            bagImage.sprite = plasticBagSprites[1];
        }
        else if (PBColor == "PBOrange")
        {
            plasticBagGreen.SetActive(false);
            plasticBagYellow.SetActive(false);

            plasticBagOrange.SetActive(true);
            bagImage.sprite = plasticBagSprites[2];
        }

        basketController.PlayerHasChosenBag();
        colorChosen = PBColor;
        userCanStartDragging = true;
        
    }
    
    public bool IsBagged()
    {
        return alreadyInBag;
    }

    public IEnumerator WaitAndClearCoroutine()
    {
        yield return new WaitForSeconds(1f);

        ClearCounter();
        // Debug.Log("Coroutine done!");
    }

    private void ClearCounter()
    {
        alreadyInBag = false;
        plasticBag.SetActive(false);
        userCanStartDragging = false;

        // Debug.Log("Counter is cleared!");
    }

    private void RemovePBFromPlayerSpace()
    {
        plasticBagGreen.SetActive(false);
        plasticBagYellow.SetActive(false);
        plasticBagOrange.SetActive(false);
    }

    public bool CanUserStartDragging()
    {
        return userCanStartDragging;
    }

    public string GetUserChosenPB()
    {
        return colorChosen;
    }
    
}
