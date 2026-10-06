using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int levelScore;
    public bool isDone;                         // player is done with the level
    public Customer[] customers;                // customers to appear in the level (3)
    public Dialogue[] smallTalkDialogues;       // smallTalk[i] -> customer[i]
    public bool[] buyingCustomers;              // if array[i] = true, customer[i] will buy

    public void StartLevel()
    {   
        levelScore = 0;
        isDone = false;

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

        StartCoroutine(RunCustomers());
        // if done
    }

    private IEnumerator RunCustomers()
    {
        foreach(Customer customerPrefab in customers)
        {

            Customer customer = Instantiate(customerPrefab);
            WaypointMover customerMover = customer.GetComponent<WaypointMover>();

            customer.isDone = false;
            customer.gameObject.SetActive(true);

            TransactionController.Instance.SetCustomer(customer);

            while (!customerMover.hasExited)
            {
                yield return null;
            }

            Debug.Log("hasExited: " + customerMover.hasExited);

            levelScore += TransactionController.Instance.GetPlayerScore();

            Destroy(customer.gameObject);
            
            // wait for 5 seconds before next customer appears
            yield return new WaitForSeconds(5f);
        }
        LevelCompleted();
    }

    public void ResetCustomers()
    {
        foreach(Customer customer in customers) 
        {
            customer.smallTalkDialogue = customer.orderDialogue = customer.endingDialogue = null;
            customer.hasSmallTalk = customer.willBuy = false;
        }
    }

    public void LevelCompleted()
    {
        isDone = true;
        LevelCompleteScreenHandler.Instance.SetStars(levelScore);
        LevelCompleteScreenHandler.Instance.ShowScreen();
        ResetCustomers(); 
    }

    

}
