using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    public bool isDone;                         // player is done with the level
    public Customer[] customers;                // customers to appear in the level (3)
    public Dialogue[] smallTalkDialogues;       // smallTalk[i] -> customer[i]
    public bool[] buyingCustomers;              // if array[i] = true, customer[i] will buy

    void Start()
    {
        
        // setup customers
        for(int i=0; i<customers.Length; i++)
        {
            // if has small talk ----------------------------------------------
            if (smallTalkDialogues[i] != null)
            {
                customers[i].hasSmallTalk = true;
                customers[i].smallTalkDialogue = smallTalkDialogues[i];
            }
            else
            {
                customers[i].hasSmallTalk = false;
            }

            // if customer will buy -------------------------------------------
            if (buyingCustomers[i])
            {
                customers[i].willBuy = true;
            }
            else
            {
                customers[i].willBuy = false;
            }
        }

        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        foreach(Customer customerPrefab in customers)
        {

            Customer customer = Instantiate(customerPrefab);

            customer.isDone = false;
            customer.gameObject.SetActive(true);

            TransactionController.Instance.SetCustomer(customer);
            customer.StartCustomer();

            while (!customer.isDone)
            {
                yield return null;
            }
            
            // wait for 5 seconds before next customer appears
            yield return new WaitForSeconds(5f);
        }
    }

    public void ResetCustomers()
    {
        foreach(Customer customer in customers) 
        {
            customer.smallTalkDialogue = customer.orderDialogue = customer.endDialogue = null;
            customer.hasSmallTalk = customer.willBuy = false;
        }
    }

    public void LevelCompleted()
    {
        isDone = true;
        ResetCustomers(); 
    }

}
