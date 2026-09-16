using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionController : MonoBehaviour
{
    private Customer customer;
    private BasketController basketController;
    private CounterDisplayController counterDisplayController;
    private ScoreChecker scoreChecker;
    private List<GameObject> itemsInBasket = new List<GameObject>();
    private GameObject[] customerOrder;

    private bool transactionDone;

    void Start()
    {
        customer = FindAnyObjectByType<Customer>();
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
        // else
        // {
        //     Debug.Log(customer.name);
        // 

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
            customer.ChangeCustomerState(CustomerState.NeutralExpression);
            counterDisplayController.DisplayPlasticBag(customerOrder.Length);
            CompleteTransaction();
            return;
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

        counterDisplayController.DisplayPlasticBag(customerOrder.Length);

        if (itemsToCheck.Count == 0)
        {
            Debug.Log("Order matches with the basket content!");
            customer.ChangeCustomerState(CustomerState.HappyExpression);
        }
        else
        {
            Debug.Log("Oops, order doesn't match.");
            customer.ChangeCustomerState(CustomerState.NeutralExpression);
        }
        
        CompleteTransaction();
    }

    private void CompleteTransaction()
    {
        basketController.ResetBasket();
        scoreChecker.ResetScore();
        StartCoroutine(ByeCustomer());

        
    }

    private IEnumerator ByeCustomer()
    {
        yield return StartCoroutine(counterDisplayController.WaitAndClearCoroutine());

        Destroy(customer.gameObject);
    }


}
