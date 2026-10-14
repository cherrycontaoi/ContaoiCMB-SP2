using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public int levelScore;
    public bool isDone;                         // player is done with the level
    public Customer[] customers;                // customers to appear in the level (3)
    public Dialogue[] smallTalkDialogues;       // smallTalk[i] -> customer[i]
    public bool[] buyingCustomers;              // if array[i] = true, customer[i] will buy
    public string[] instructionsFileNames = new string[3];
    public LevelType levelType;
    public Dictionary<Rule, bool> activeRules = new Dictionary<Rule, bool>
    {
        { Rule.ItlogLast, false},
        { Rule.ItlogAndBottle, false },
        { Rule.ItlogInCornerSlot, false },
        { Rule.CannedFirst, false },
        { Rule.GlassBottles, false },
        { Rule.DetergentProduce, false },
        { Rule.TooMuchUmami, false },
        { Rule.ProperPlasticBag, false },
        { Rule.Pairings, false },
        { Rule.ContentRules, false },
    };
    public bool[] activeRulesArr = new bool[10];

    public void StartLevel()
    {   
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            levelScore = 300;
        }
        else
        {
            levelScore = 0;
        }

        isDone = false;
        
        // set text content based on difficulty
        switch (PlayerData.Instance.playerPuzzleLevels)
        {
            case Difficulty.Easy:
                OtherScreensController.Instance.SetInstructions(instructionsFileNames[0]);
                break;

            case Difficulty.Medium:
                OtherScreensController.Instance.SetInstructions(instructionsFileNames[1]);
                break;
            
            case Difficulty.Hard:
                OtherScreensController.Instance.SetInstructions(instructionsFileNames[2]);
                break;

            // only 1 text for tests
            case Difficulty.Test:
                OtherScreensController.Instance.SetInstructions(instructionsFileNames[0]);
                break;
        }
        // OtherScreensController.Instance.SetInstructions(instructionsFileName);
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
        PlayerData.Instance.UpdatePlayerLevel();
        PlayerData.Instance.UpdatePlayerInfo();

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

            while (CounterDisplayController.Instance.MoneyIsStillOnCounter())
            {
                yield return null;
            }

            levelScore += TransactionController.Instance.GetPlayerScore();
            TransactionController.Instance.ResetTransactionScore();

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
        activeRules[Rule.ItlogLast] = activeRulesArr[0];
        activeRules[Rule.ItlogAndBottle] = activeRulesArr[1];
        activeRules[Rule.ItlogInCornerSlot] = activeRulesArr[2];
        activeRules[Rule.CannedFirst] = activeRulesArr[3];
        activeRules[Rule.GlassBottles] = activeRulesArr[4];
        activeRules[Rule.DetergentProduce] = activeRulesArr[5];
        activeRules[Rule.TooMuchUmami] = activeRulesArr[6];
        activeRules[Rule.ProperPlasticBag] = activeRulesArr[7];
        activeRules[Rule.Pairings] = activeRulesArr[8];
        activeRules[Rule.ContentRules] = activeRulesArr[9];
    }
    
}

public enum LevelType
{
    PreTest,
    Training,
    PostTest
}
