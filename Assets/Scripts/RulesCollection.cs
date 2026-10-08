using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RulesCollection : MonoBehaviour
{
    private ScoreChecker scoreChecker;
    private Customer customer;
    private List<int> cornerSlots = new List<int>{0,2,6,8};    // corners

    void Start()
    {
        scoreChecker = gameObject.GetComponent<ScoreChecker>();
    }

    public void ApplyRule(GameObject itemToCheck)
    {
        customer = FindAnyObjectByType<Customer>();

        ItemType itemType = itemToCheck.GetComponent<Item>().GetItemType();

        if (itemType == ItemType.PartOfPair)
        {
            ApplyAdjacencyRules(itemToCheck);       // glass bottle rules not being applied to part of pair yet
        }

        else if (itemType == ItemType.Constricted)
        {
            ApplyIndividualRules(itemToCheck);
        }

        else    // nothing to do, no rule  -> Free item
        {
            return;
        }
    }

    private void ApplyAdjacencyRules(GameObject itemToCheck)
    {
        List<GameObject> neighbors = scoreChecker.GetNeighboringItems(itemToCheck);
        List<string> nameOfPossiblePairs = itemToCheck.GetComponent<Item>().GetMatchingItemNames();
        
        foreach(GameObject neighbor in neighbors)
        {
            if (nameOfPossiblePairs.Contains(neighbor.name))
            {
                return;
            }
        }

        // if matching item not found
        Debug.Log("The matching item for "+ itemToCheck.name +" is not found!");
        customer.orderErrors["noMatchingItem"] = true;
        scoreChecker.DeductScore(5);
    }

    private void ApplyIndividualRules(GameObject itemToCheck)
    {
        if (itemToCheck.name == "Itlog") ApplyItlogRules(itemToCheck);
        else if (itemToCheck.name == "Ajinomoto" && RulesTracker.Instance.activeRulesInLevel[Rule.TooMuchUmami]) ApplyAjinomotoRules(itemToCheck);
        else if (itemToCheck.GetComponent<Item>().IsItGlassBottle() && RulesTracker.Instance.activeRulesInLevel[Rule.GlassBottles]) ApplyBottleRules(itemToCheck);
        else if (itemToCheck.CompareTag("Detergent") && RulesTracker.Instance.activeRulesInLevel[Rule.DetergentProduce]) ApplyDetergentRules(itemToCheck);
    }

    public void ApplyRulesOnItemsOrder(List<GameObject> basketQueue)
    {
        customer = FindAnyObjectByType<Customer>();

        int itlogCount = scoreChecker.GetItlogCount();
        int cannedCount = scoreChecker.GetCannedCount();

        // egg should be the last to be put sa bag
        // iterate through the array from the last index
        if (itlogCount > 0)
        {
            int endIndex = basketQueue.Count - itlogCount;
            for(int i = basketQueue.Count-1 ; i >= endIndex; i--)
            {
                if (basketQueue[i].name != "Itlog")
                {
                    Debug.Log("Eggs should be placed last!");
                    customer.orderErrors["itlogNotLast"] = true;
                    scoreChecker.DeductScore(5);
                }
            }
        }

        // canned should be placed first
        if (cannedCount > 0)
        {

            for (int i=0; i<cannedCount; i++)
            {
                if (!basketQueue[i].CompareTag("Canned"))
                {
                    Debug.Log("Canned should be placed first!");
                    customer.orderErrors["cannedNotFirst"] = true;
                    scoreChecker.DeductScore(5);
                }
            }
        }
    }

    private void ApplyItlogRules(GameObject itemToCheck)
    {
        if (RulesTracker.Instance.activeRulesInLevel[Rule.ItlogInCornerSlot])
        {
            customer = FindAnyObjectByType<Customer>();
            // RULE 1: Itlog cannot be placed next to Glass Bottles
            List<GameObject> neighbors = scoreChecker.GetNeighboringItems(itemToCheck);

            foreach (GameObject neighbor in neighbors)
            {
                if (neighbor.GetComponent<Item>().IsItGlassBottle())
                {
                    Debug.Log("Itlog cannot be placed next to a glass bottle!");
                    customer.orderErrors["itlogAndBottle"] = true;
                    scoreChecker.DeductScore(5);
                }
            }
        }
        
        if (RulesTracker.Instance.activeRulesInLevel[Rule.ItlogAndBottle])
        {
            // RULE 2: Itlog cannot be placed in a corner slot
            if (cornerSlots.Contains(itemToCheck.GetComponent<Item>().GetSlotID()))
            {
                Debug.Log("Itlog cannot be placd at the corner!");
                customer.orderErrors["itlogAtCorner"] = true;
                scoreChecker.DeductScore(5);
            }
        }
        
    }

    private void ApplyBottleRules(GameObject itemToCheck)
    {
        customer = FindAnyObjectByType<Customer>();
        // RULE: Glass bottle cannot be placed next to a glass bottle
        List<GameObject> neighbors = scoreChecker.GetNeighboringItems(itemToCheck);

        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor.GetComponent<Item>().IsItGlassBottle())
            {
                Debug.Log("A glass bottle can't be put next to another one!");
                customer.orderErrors["bottleAndBottle"] = true;
                scoreChecker.DeductScore(5);
            }
        }
    }

    private void ApplyDetergentRules(GameObject itemToCheck)
    {
        customer = FindAnyObjectByType<Customer>();
        // RULE: Detergent cannot be put next to produce
        List<GameObject> neighbors = scoreChecker.GetNeighboringItems(itemToCheck);

        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor.CompareTag("Produce"))
            {
                Debug.Log("Detergent can't be put next to produce!");
                customer.orderErrors["detergentAndProduce"] = true;
                scoreChecker.DeductScore(5);
            }
        }
    }

    private void ApplyAjinomotoRules(GameObject itemToCheck)
    {
        customer = FindAnyObjectByType<Customer>();
        // RULE: Ajinomoto item cannot be put next to MagicSarap item
        List<GameObject> neighbors = scoreChecker.GetNeighboringItems(itemToCheck);

        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor.name == "MagicSarap")
            {
                Debug.Log("Ajinomoto can't be put next to Magic Sarap");
                customer.orderErrors["ajinomotoAndMagicSarap"] = true;
                scoreChecker.DeductScore(5);
            }
        }
    }

    
}

public enum Rule
{
    ItlogAndBottle,
    ItlogInCornerSlot,
    GlassBottles,
    DetergentProduce,
    TooMuchUmami
}