using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int levelScore;
    public bool isDone;                         // player is done with the level
    public Customer[] customers;                // customers to appear in the level (3)
    public Dialogue[] smallTalkDialogues;       // smallTalk[i] -> customer[i]
    public bool[] buyingCustomers;              // if array[i] = true, customer[i] will buy
    public string instructionsFileName;
    public Dictionary<Rule, bool> activeRules = new Dictionary<Rule, bool>
    {
        { Rule.ItlogAndBottle, false },
        { Rule.ItlogInCornerSlot, false },
        { Rule.GlassBottles, false },
        { Rule.DetergentProduce, false },
        { Rule.TooMuchUmami, false },
    };
    public bool[] activeRulesArr = new bool[5];

    public void StartLevel()
    {   
        levelScore = 0;
        isDone = false;

        OtherScreensController.Instance.SetInstructions(instructionsFileName);
        OtherScreensController.Instance.ShowStartAnimation();
        OtherScreensController.Instance.CloseStore();

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
        SetRulesDictionary();
        RulesTracker.Instance.SetRules(activeRules);

        StartCoroutine(RunCustomers());
        
    }

    private IEnumerator RunCustomers()
    {
        while (!OtherScreensController.Instance.storeIsOpen)
        {
            yield return null;
        }

        // wait muna
        yield return new WaitForSeconds(5f);

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
        yield return StartCoroutine(LevelCompleted());
    }

    public void ResetCustomers()
    {
        foreach(Customer customer in customers) 
        {
            customer.smallTalkDialogue = customer.orderDialogue = customer.endingDialogue = null;
            customer.hasSmallTalk = customer.willBuy = false;
        }
    }

    public IEnumerator LevelCompleted()
    {
        isDone = true;
        OtherScreensController.Instance.ShowTransitionDialogue(true);
        OtherScreensController.Instance.SetStars(levelScore);

        yield return new WaitForSeconds(3f);

        OtherScreensController.Instance.ShowTransitionDialogue(false);
        OtherScreensController.Instance.ShowScreen();
        ResetCustomers(); 

        // LevelController.Instance.StartNextLevel();
    }

    private void SetRulesDictionary()
    {
        activeRules[Rule.ItlogAndBottle] = activeRulesArr[0];
        activeRules[Rule.ItlogInCornerSlot] = activeRulesArr[1];
        activeRules[Rule.GlassBottles] = activeRulesArr[2];
        activeRules[Rule.DetergentProduce] = activeRulesArr[3];
        activeRules[Rule.TooMuchUmami] = activeRulesArr[4];
    }
    
}
