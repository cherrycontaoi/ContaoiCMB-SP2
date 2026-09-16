using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RulesCollection : MonoBehaviour
{

    private ScoreChecker scoreChecker;
    private bool foundBawang, foundChickenNoodles, foundSuka, foundChips;
    private int ilangSibuyasMatched, ilangChickenNoodlesMatched, ilangSukaMatched, ilangSodaMatched;

    void Start()
    {
        scoreChecker = gameObject.GetComponent<ScoreChecker>();
    }

    public void ApplyRule(GameObject item)
    {
        if (item.name == "Itlog")
        {
            ApplyRulesOnItlog(item);
        }

        if (item.name == "Sibuyas")
        {
            ApplyRulesOnSibuyas(item);
        }

        if (item.name == "Toyo")
        {
            ApplyRulesOnToyo(item);
        }

        if (item.CompareTag("Glass Bottle"))
        {
            ApplyRulesOnBottle(item);
        }

        if (item.CompareTag("Soda"))
        {
            ApplyRulesOnSoda(item);
        }

        if (item.CompareTag("Detergent"))
        {
            ApplyRulesOnDetergent(item);
        }

    }

    public void ApplyRulesOnItemsOrder(List<GameObject> basketQueue)
    {
        // egg should be the last to be put sa bag
        // iterate through the array from the last index
        if (scoreChecker.ilangItlog > 0)
        {
            int endIndex = basketQueue.Count - scoreChecker.ilangItlog;
            for(int i = basketQueue.Count-1 ; i >= endIndex; i--)
            {
                if (basketQueue[i].name != "Itlog")
                {
                    Debug.Log("Eggs should be placed last!");
                    scoreChecker.DeductScore(5);
                }
            }
        }

        // canned should be placed first
        if (scoreChecker.ilangCanned > 0)
        {

            for (int i=0; i<scoreChecker.ilangCanned; i++)
            {
                if (basketQueue[i].CompareTag("Canned"))
                {
                    Debug.Log("Canned should be placed first!");
                    scoreChecker.DeductScore(5);
                }
            }
        }
    }

    // adjacent = matched
    // get minimum of the two items to get the required amount of matches
    public void ApplyMatchingRules()
    {
        int requiredMatches, mismatches;

        // for egg to noodles matching
        requiredMatches = Mathf.Min(scoreChecker.ilangItlog, scoreChecker.ilangChickenNoodles);
        if (ilangChickenNoodlesMatched < requiredMatches)
        {
            mismatches = requiredMatches - ilangChickenNoodlesMatched;
            for (int i=0; i< mismatches; i++)
            {
                Debug.Log("Itlog should be placed next to chicken noodle!");
                scoreChecker.DeductScore(5);
            }
        }

        // for sibuyas to garlic matching
        requiredMatches = Mathf.Min(scoreChecker.ilangBawang, scoreChecker.ilangSibuyas);
        if (ilangSibuyasMatched < requiredMatches)
        {
            mismatches =  requiredMatches - ilangSibuyasMatched;
            for (int i=0; i<mismatches; i++)
            {
                Debug.Log("Sibuyas should be placed next to bawang!");
                scoreChecker.DeductScore(5);
            }
        }

        // for toyo to suka matching
        requiredMatches = Mathf.Min(scoreChecker.ilangToyo, scoreChecker.ilangSuka);
        if (ilangSukaMatched < requiredMatches)
        {
            mismatches =  requiredMatches - ilangSukaMatched;
            for (int i=0; i<mismatches; i++)
            {
                Debug.Log("Suka should be placed next to Toyo!");
                scoreChecker.DeductScore(5);
            }
        }

        // for soda to chips matching
        requiredMatches = Mathf.Min(scoreChecker.ilangSoda, scoreChecker.ilangChips);
        if (ilangSodaMatched < requiredMatches)
        {
            mismatches =  requiredMatches - ilangSodaMatched;
            for (int i=0; i<mismatches; i++)
            {
                Debug.Log("Soda should be placed next to Chips!");
                scoreChecker.DeductScore(5);
            }
        }
    }

    public void ResetItemCounters()
    {
        ilangSibuyasMatched = ilangChickenNoodlesMatched = ilangSodaMatched = ilangSukaMatched = 0;
    }

    // itlog cannot be put next to glass bottle (tag)
    // itlog cannot be placed at the corners
    // if there is also chicken noodle, itlog must be adjacent to it
    private void ApplyRulesOnItlog(GameObject item)
    {
        List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);
        int[] cornerSlotIDs = {0,2,6,8};


        // check if there's a bottle in neighbors
        foreach(GameObject neighboringItem in neighboringItems)
        {
            if (neighboringItem.CompareTag("Glass Bottle"))
            {
                Debug.Log("Itlog cannot be placed next to a glass bottle!");
                scoreChecker.DeductScore(5);    // deduct 5
            }
        }

        // check if there's chicken noodle in neighbors (1:1 ratio)
        if (scoreChecker.ilangChickenNoodles > 0)
        {
            // if (scoreChecker.ilangChickenNoodles > ilangChickenNoodlesMatched)
            // {
                foreach(GameObject neighboringItem in neighboringItems)
                {
                    if(neighboringItem.name == "LuckyMeChicken")
                    { 
                        ilangChickenNoodlesMatched++;
                        break;
                            
                    }   
                }
            // }
        }
        
        // check if itlog is at the corner
        if (cornerSlotIDs.Contains(item.GetComponent<Item>().GetSlotID()))
        {
            Debug.Log("Itlog cannot be placed at the corner!");
            scoreChecker.DeductScore(5);
        }
    }

    private void ApplyRulesOnSibuyas(GameObject item)
    {
        // sibuyas and bawang should be next to each other (1:1)
        if (scoreChecker.ilangBawang > 0)
        {
            // if (scoreChecker.ilangBawang > ilangBawangMatched)
            // {
                List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);
                foreach(GameObject neighboringItem in neighboringItems)
                {
                    if(neighboringItem.name == "Bawang")
                    {
                        ilangSibuyasMatched++;
                        break;
                    }
                }
            // }
        }
    }

    private void ApplyRulesOnBottle(GameObject item)
    {
        List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);

        // a glass bottle cannot be put next to another bottle
        foreach(GameObject neighboringItem in neighboringItems)
        {
            if(neighboringItem.CompareTag("Glass Bottle"))
            {
                Debug.Log("Glass bottles cannot be put next to each other!");
                scoreChecker.DeductScore(5);
            }
        }

    }

    private void ApplyRulesOnSoda(GameObject item)
    {
        if (scoreChecker.ilangChips > 0)
        {
            // if (scoreChecker.ilangChips > ilangChipsMatched)
            // {
                List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);

                foreach(GameObject neighboringItem in neighboringItems)
                {
                    if (neighboringItem.CompareTag("Chips")) // (1:1)
                    {
                        // foundChips = true;
                        ilangSodaMatched++;
                        break;
                    }
                }

                // if (!foundChips)
                // {
                //     Debug.Log("Chips should be placed next to Soda!");
                //     scoreChecker.DeductScore(5);
                // }

                // foundChips = false;

            // }
        }
    }

    private void ApplyRulesOnDetergent(GameObject item)
    {
        if (scoreChecker.mayProduceBa)
        {
            List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);

            foreach(GameObject neighboringItem in neighboringItems)
            {
                if (neighboringItem.CompareTag("Produce"))
                {
                    Debug.Log("Produce items cannot be put next to Detergent!");
                    scoreChecker.DeductScore(5);
                }
            }    
        }
        
    }

    private void ApplyRulesOnToyo(GameObject item)
    {
        if (scoreChecker.ilangSuka > 0)
        {
            // if (scoreChecker.ilangSuka > ilangSukaMatched)
            // {
                List<GameObject> neighboringItems = scoreChecker.GetNeighboringItems(item);
                foreach(GameObject neighboringItem in neighboringItems)
                {
                    if(neighboringItem.name == "Suka")
                    {
                        // foundSuka = true;
                        ilangSukaMatched++;
                        break;
                    }
                }

                // if (!foundSuka)
                // {
                //     Debug.Log("Suka should be next to Toyo!");
                //     scoreChecker.DeductScore(5);
                // }

                // // reset
                // foundSuka = false;
            // }
            
        }
    }

}
