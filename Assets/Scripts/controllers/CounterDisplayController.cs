using System.Collections;
using UnityEngine;

public class CounterDisplayController : MonoBehaviour
{

    public static CounterDisplayController Instance { get; private set; }

    [SerializeField] private GameObject plasticBag;
    [SerializeField] private Sprite[] plasticBagSprites = new Sprite[3];
    [SerializeField] private GameObject givePanel;
    [SerializeField] private GameObject plasticBagGreen;
    [SerializeField] private GameObject plasticBagYellow;
    [SerializeField] private GameObject plasticBagOrange;
    [SerializeField] private BasketController basketController;

    [SerializeField] private GameObject money;
    [SerializeField] private Sprite[] easyMoneySprites = new Sprite[3];
    [SerializeField] private Sprite[] mediumMoneySprites = new Sprite[3];
    [SerializeField] private Sprite[] hardMoneySprites = new Sprite[3];
    
    private bool moneyOnCounter = false;
    private bool alreadyInBag;
    private bool userCanStartDragging = false;
    private string colorChosen;
    // private int noOfOrders;
    private SpriteRenderer bagImage;

    public bool customerIsChecking; // customer has the bag already

    private void Start()
    {
        basketController = GetComponent<BasketController>();
        bagImage = plasticBag.GetComponent<SpriteRenderer>();
        userCanStartDragging = false;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    public void DisplayMoney()
    {
        int index = Random.Range(0,3);
        switch (PlayerData.Instance.playerPuzzleLevels)
        {
            case Difficulty.Easy:
                money.GetComponent<SpriteRenderer>().sprite = easyMoneySprites[index];
                break;

            case Difficulty.Medium:
                money.GetComponent<SpriteRenderer>().sprite = mediumMoneySprites[index];
                break;

            case Difficulty.Hard:
                money.GetComponent<SpriteRenderer>().sprite = hardMoneySprites[index];
                break;
            
            case Difficulty.Test:
                money.GetComponent<SpriteRenderer>().sprite = hardMoneySprites[index];
                break;
        }

        money.SetActive(true);
        moneyOnCounter = true;
    }
    
    public void GetMoney()
    {
        if (!moneyOnCounter) return;

        SoundFXController.Play("GetMoney");
        moneyOnCounter = false;
        money.SetActive(false);

        Debug.Log("Payment is collected!");
    }
    
    public void DisplayPlasticBag(int noOfOrders)
    {        
        SoundFXController.Play("Plastic");
        alreadyInBag = true;
        customerIsChecking = true;

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
        if(PauseController.IsGamePaused || customerIsChecking || customer == null || !customer.willBuy || !customer.currentlyBuying || !customer.GetComponent<WaypointMover>().hasStoppedAtStore || Pencil.Instance.isActive) return;

        SoundFXController.Play("Plastic");
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

        // DisplayMoney(); // display money after customer gets the bag
        // Debug.Log("Counter is cleared!");
    }

    public void RemovePBFromPlayerSpace()
    {
        plasticBagGreen.SetActive(false);
        plasticBagYellow.SetActive(false);
        plasticBagOrange.SetActive(false);
    }

    public bool CanUserStartDragging()
    {
        return userCanStartDragging;
    }

    public bool MoneyIsStillOnCounter()
    {
        return moneyOnCounter;
    }

    public string GetUserChosenPB()
    {
        return colorChosen;
    }
    
}
