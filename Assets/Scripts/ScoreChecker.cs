using System.Collections.Generic;
using UnityEngine;

public class ScoreChecker : MonoBehaviour
{
    private BasketController basketController;
    [SerializeField] private RulesCollection rulesCollection;
    private GameObject[] placedItems;
    private List<GameObject> actualItemsOnly = new List<GameObject>();
    // private 
    [SerializeField] private string[] TagsWithConstraints;          // for rules e.g. 'Itlog'

    [SerializeField] private int playerScore = 100;
    public bool mayBawangBa, mayChickenNoodlesBa, maySukaBa, mayChipsBa, mayProduceBa;
    public int ilangCanned, ilangItlog, ilangChickenNoodles, ilangBawang, ilangSibuyas;
    public int ilangChips, ilangSoda, ilangSuka, ilangToyo;
    
    void Start()
    {
        basketController = gameObject.GetComponent<BasketController>();
        rulesCollection = gameObject.GetComponent<RulesCollection>();
    }

    private void MeronBa()
    {
        actualItemsOnly = basketController.GetActualItemsInBasket();

        foreach(GameObject item in actualItemsOnly)
        {
            if (item.name == "Bawang")
            {
                mayBawangBa = true;
                ilangBawang++;
            }

            if (item.name == "Sibuyas")
            {
                ilangSibuyas++;
            }

            if (item.name == "Itlog")
            {
                ilangItlog++;
            }

            if (item.name == "LuckyMeChicken")
            {
                mayChickenNoodlesBa = true;
                ilangChickenNoodles++;
            }

            if (item.name == "Suka")
            {
                maySukaBa = true;
                ilangSuka++;
            }

            if (item.name == "Toyo")
            {
                ilangToyo++;
            }
            
            if (item.CompareTag("Chips"))
            {
                mayChipsBa = true;
                ilangChips++;
            }

            if (item.CompareTag("Soda"))
            {
                ilangSoda++;
            }

            if (item.CompareTag("Produce"))
            {
                mayProduceBa = true;
            }

            if (item.CompareTag("Canned"))
            {
                ilangCanned++;
            }

        }
    }

    public void CheckItems()
    {
        placedItems = basketController.GetFullBasket();     // need full basket for checking neighbors

        MeronBa();

        foreach (GameObject item in placedItems)
        {
            if (item!= null)
            {
                rulesCollection.ApplyRule(item);
            }
        }

        rulesCollection.ApplyRulesOnItemsOrder(actualItemsOnly);
        rulesCollection.ApplyMatchingRules();


        // foreach (GameObject item in placedItems)
        // {
        //     Debug.Log("The Neighboring Items of "+ item.name + "are: ");
        //     GetNeighboringItems(item);
        // }
        
        // reset
        rulesCollection.ResetItemCounters();
        mayBawangBa = mayChickenNoodlesBa = maySukaBa = mayChipsBa = mayProduceBa = false;
        ilangItlog = ilangCanned = ilangChickenNoodles = ilangBawang = ilangSibuyas = ilangSuka = ilangToyo = ilangChips = ilangSoda = 0;

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

        // foreach(GameObject neighboringItem in neighboringItems)
        // {
        //     Debug.Log(neighboringItem.name);
        // }
    }

    public void DeductScore(int deduction)
    {
        playerScore -= deduction;
        Debug.Log("Deducted a score of: "+ deduction);
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
