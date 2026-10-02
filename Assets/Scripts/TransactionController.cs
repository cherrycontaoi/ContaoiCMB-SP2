using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionController : MonoBehaviour
{
    public static TransactionController Instance { get; private set; } // Singleton
    public Customer customer;
    private PlayerData playerData;
    private BasketController basketController;
    private CounterDisplayController counterDisplayController;
    private ScoreChecker scoreChecker;
    private List<GameObject> itemsInBasket = new List<GameObject>();
    private GameObject[] customerOrder;

    void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        basketController = gameObject.GetComponent<BasketController>();
        counterDisplayController = gameObject.GetComponent<CounterDisplayController>();
        scoreChecker = gameObject.GetComponent<ScoreChecker>();

        if (basketController == null)
        {
            Debug.Log("No basket found");
        }


        if (customer == null)
        {
            Debug.Log("No customer found!");
            return;
        }

    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    public void SetCustomer(Customer newCustomer)
    {
        customer = newCustomer;
    }

    public void Checkout()
    {
        scoreChecker.CheckItems();

        itemsInBasket = basketController.GetActualItemsInBasket();
        customerOrder = customer.order.GetItems();

        // Debug.Log("Customer ordered " + customerOrder.Length + " items");
        // Debug.Log("The basket has " + itemsInBasket.Length + " items");

        List<GameObject> itemsToCheck = new List<GameObject>(customerOrder);

        // Debug.Log("The items in the basket are:");

        // foreach(GameObject item in itemsInBasket)
        // {
        //     Debug.Log(item.name);
        // }

        // Debug.Log("The orders of the customer are:");
        // foreach(GameObject item in customerOrder)
        // {
        //     Debug.Log(item.name);
        // }

        
        

        if (itemsInBasket.Count != customerOrder.Length)
        {
            Debug.Log("The number of items in the basket doesn't match with the customer's order!");
            customer.orderErrors["notSameCount"] = true;
            scoreChecker.DeductScore(5);
            // customer.ChangeCustomerState(CustomerState.NeutralExpression);
            // counterDisplayController.DisplayPlasticBag(customerOrder.Length);
            // CompleteTransaction();
            // return;
        }


        // compare by name, check if the orders match with the basket content
        foreach(GameObject item in itemsInBasket)
        {
            GameObject matchedItem = null;

            foreach(GameObject toCheck in itemsToCheck)
            {
                if (toCheck.name == item.name)
                {
                    matchedItem = toCheck;
                    break;
                }
            }

            if (matchedItem != null)
            {
                itemsToCheck.Remove(matchedItem);
            }
        }

        ApplyPlasticBagRule();
        counterDisplayController.DisplayPlasticBag(customerOrder.Length);

        // if (itemsToCheck.Count == 0)
        // {
        //     Debug.Log("Order matches with the basket content!");
        //     customer.ChangeCustomerState(CustomerState.HappyExpression);
        // }
        // else
        // {
        //     Debug.Log("Oops, order doesn't match.");
        //     customer.ChangeCustomerState(CustomerState.NeutralExpression);
        // }
        
        // customer.GetComponent<Order>().ResetItemMatchings();
        customer.order.ResetItemMatchings();
        CompleteTransaction();
    }

    private void CompleteTransaction()
    {

        int playerScore = scoreChecker.GetPlayerScore();
        Debug.Log("The player's score for this level: " + playerScore);
        if (playerScore < 100)
        {
            Debug.Log("There are some mistakes on the customer's order");
            customer.ChangeCustomerState(CustomerState.NeutralExpression);
        }

        else
        {
            Debug.Log("Customer is happy!");
            customer.ChangeCustomerState(CustomerState.HappyExpression);
        }


        basketController.ResetBasket();
        scoreChecker.ResetScore();

        StartCoroutine(ByeCustomer());
        
        playerData.AddFinishedTransaction(playerScore);
        
    }

    private IEnumerator ByeCustomer()
    {
        yield return StartCoroutine(counterDisplayController.WaitAndClearCoroutine());

        customer.StartEndingDialogue();
        // Destroy(customer.gameObject);
    }

    private void ApplyPlasticBagRule()
    {

        string playerChosenBag = counterDisplayController.GetUserChosenPB();
        string correctPlasticBag;

        if (customerOrder.Length <= 3)
        {
            correctPlasticBag = "PBGreen";
        }
        else if (customerOrder.Length > 3 && customerOrder.Length <= 6)
        {
            correctPlasticBag = "PBYellow";
        }
        else
        {
            correctPlasticBag = "PBOrange";
        }

        if (correctPlasticBag != playerChosenBag)
        {
            Debug.Log("The correct plastic bag to use is "+ correctPlasticBag + ", not " + playerChosenBag);
            customer.orderErrors["wrongPlasticBag"] = true;
            scoreChecker.DeductScore(5);
        }
    }



}
