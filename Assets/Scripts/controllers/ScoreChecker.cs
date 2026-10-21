using System.Collections.Generic;
using UnityEngine;

public class ScoreChecker : MonoBehaviour
{
    private BasketController basketController;
    private Customer customer;
    [SerializeField] private RulesCollection rulesCollection;
    private GameObject[] placedItems;
    private List<GameObject> actualItemsOnly = new List<GameObject>();
    
    [SerializeField] private int playerScore = 100;

    // to be used by dialogueBuilder for endDialogue

    
    void Start()
    {
        basketController = gameObject.GetComponent<BasketController>();
        rulesCollection = gameObject.GetComponent<RulesCollection>();
        customer = FindAnyObjectByType<Customer>();
        actualItemsOnly = basketController.GetActualItemsInBasket();
    }

    public void CheckItems()
    {
        placedItems = basketController.GetFullBasket();     // need full basket for checking neighbors

        foreach(GameObject item in placedItems)
        {
            if (item != null)
            {
                rulesCollection.ApplyRule(item);
            }
        }

        rulesCollection.ApplyRulesOnItemsOrder(actualItemsOnly);

    }

    public List<GameObject> GetNeighboringItems(GameObject item)
    {
        Item currentItem = item.GetComponent<Item>();
        int itemSlotID = currentItem.GetSlotID();
        List<GameObject> neighboringItems = new List<GameObject>();      // neighbors of an item in the "grid"

        int row = itemSlotID / 3;
        int col =  itemSlotID % 3;

        if (row>0 && placedItems[itemSlotID-3] != null)
        {
            neighboringItems.Add(placedItems[itemSlotID-3]);
        }
        
        if (row<2 && placedItems[itemSlotID+3] != null)
        {
            neighboringItems.Add(placedItems[itemSlotID+3]);
        }

        if (col>0 && placedItems[itemSlotID-1] != null) 
        {
            neighboringItems.Add(placedItems[itemSlotID-1]);
        }

        if (col<2 && placedItems[itemSlotID+1] != null) 
        {
            neighboringItems.Add(placedItems[itemSlotID+1]);
        }

        return neighboringItems;
    }

    public int GetItlogCount()
    {
        int itlogCount = 0;

        foreach (GameObject item in actualItemsOnly)
        {
            if (item.name == "Itlog")
            {
                itlogCount++;
            }
        }

        return itlogCount;
    }

    public int GetCannedCount()
    {
        int cannedCount = 0;

        foreach (GameObject item in actualItemsOnly)
        {
            if (item.CompareTag("Canned"))
            {
                cannedCount++;
            }
        }

        return cannedCount;

    }

    public void DeductScore(int deduction)
    {
        playerScore -= deduction;
        Debug.Log("Deducted a score of: "+ deduction);
    }

    public void ResetErrorTally()
    {
        foreach(string errorKey in customer.orderErrors.Keys)
        {
            customer.orderErrors[errorKey] = false;
        }
    }

    public void ResetScore()
    {
        playerScore = 100;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public List<GameObject> GetBasketContent()
    {
        return actualItemsOnly;
    }
}
