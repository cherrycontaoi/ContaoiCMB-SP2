using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public List<int> levelScores = new List<int>();
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
        // FadeTransition();

        StartCoroutine(StartLevelTransition());
        
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            levelScore = 300;
        }
        else
        {
            levelScore = 0;
        }

        SoundFXController.Play("StartOfLevel");

        isDone = false;
        NotesController.Instance.ClearNotes();
        CounterDisplayController.Instance.RemovePBFromPlayerSpace();
        
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
        
        // setup customers
        for(int i=0; i<customers.Length; i++)
        {

            Customer customer = Instantiate(customers[i]);
            customer.smallTalkDialogue = null;
            customer.orderDialogue = null;
            customer.endingDialogue = null;

            // if has small talk ----------------------------------------------
            if (smallTalkDialogues[i] != null)
            {
                customer.hasSmallTalk = true;
                customer.smallTalkDialogue = smallTalkDialogues[i];
            }
            else
            {
                customer.hasSmallTalk = false;
            }

            // if customer will buy -------------------------------------------
            if (buyingCustomers[i])
            {
                customer.willBuy = true;
            }
            else
            {
                customer.willBuy = false;
            }

            WaypointMover customerMover = customer.GetComponent<WaypointMover>();
            customer.isDone = false;
            customer.gameObject.SetActive(true);

            TransactionController.Instance.SetCustomer(customer);
            CounterDisplayController.Instance.customerIsChecking = false;

            while (!customerMover.hasExited)
            {
                yield return null;
            }

            while (CounterDisplayController.Instance.MoneyIsStillOnCounter())
            {
                yield return null;
            }

            levelScores.Add(TransactionController.Instance.GetPlayerScore());
            levelScore += TransactionController.Instance.GetPlayerScore();
            TransactionController.Instance.ResetTransactionScore();

            Destroy(customer.gameObject);
            
            // wait for 5 seconds before next customer appears
            yield return new WaitForSeconds(5f);

        }

        yield return StartCoroutine(LevelCompleted());

        // foreach(Customer customerPrefab in customers)
        // {

        //     Customer customer = Instantiate(customerPrefab);
        //     WaypointMover customerMover = customer.GetComponent<WaypointMover>();

        //     customer.isDone = false;
        //     customer.gameObject.SetActive(true);

        //     TransactionController.Instance.SetCustomer(customer);

        //     while (!customerMover.hasExited)
        //     {
        //         yield return null;
        //     }

        //     while (CounterDisplayController.Instance.MoneyIsStillOnCounter())
        //     {
        //         yield return null;
        //     }

        //     levelScore += TransactionController.Instance.GetPlayerScore();
        //     TransactionController.Instance.ResetTransactionScore();

        //     Destroy(customer.gameObject);
            
        //     // wait for 5 seconds before next customer appears
        //     yield return new WaitForSeconds(5f);
        // }
    }

    public void ResetCustomers()
    {
        foreach(Customer customer in customers) 
        {
            customer.smallTalkDialogue = customer.orderDialogue = customer.endingDialogue = null;
            customer.hasSmallTalk = customer.willBuy = false;
        }
    }

    private async void FadeTransition()
    {
        await ScreenFader.Instance.FadeOut();
        await ScreenFader.Instance.FadeIn();
    }

    private IEnumerator StartLevelTransition()
    {
        ScreenFader.Instance.StartFaded();
        var fadeTask = ScreenFader.Instance.FadeIn();

        while (!fadeTask.IsCompleted)
        {
            yield return null;
        }
    }

    public IEnumerator LevelCompleted()
    {
        isDone = true;

        PlayerData.Instance.AddCompletedLevel(levelScore, levelScores, levelType);
        PlayerData.Instance.UpdatePlayerLevel();

        PlayerData.Instance.lastFinishedLevel = LevelController.Instance.currentLevelIndex;
        OtherScreensController.Instance.ShowTransitionDialogue(true);
        OtherScreensController.Instance.SetStars(levelScore);

        yield return new WaitForSeconds(3f);

        OtherScreensController.Instance.ShowTransitionDialogue(false);
        OtherScreensController.Instance.ShowScreen();
        // ResetCustomers(); 

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
