using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsToBuy;
    private GameObject[] possibleSolution = new GameObject[9];      // nine slots
    
    private List<int> cornerSlotsID = new List<int>{0,2,6,8};    // cant place itlog here
    // private int numOfItems, placedItems, index,       // max num of items is 9
    //             numOfItlog, numOfBawang, numOfSoda, numOfToyo,
    //             numOfChickenNoodles, numOfSibuyas, numOfChips, numOfSuka,
    //             numOfItlogNoodlesPairs, numOfBawangSibuyasPairs, numOfSodaChipsPairs, numOfToyoSukaPairs,
    //             ilangChickenNoodlesMatched, ilangSibuyasMatched, ilangSukaMatched, ilangChipsMatched;

    private int itlogLimit = 5;
    private bool isSolvable;
    // private GameObject matchedItem;

    [SerializeField] private Dictionary<int, bool> pairTracker = new Dictionary<int, bool>(); // tracks which pairs are active
    private GameObject[] paninda;   // all the paninda
    private List<GameObject> itemsInPair = new List<GameObject>();      // itemtype = partofpair
    private List<GameObject> constrictedItems = new List<GameObject>(); // individual items w/ rules
    private List<GameObject> freeItems = new List<GameObject>();        // no rules, can be placed anywhere
    private List<GameObject> listToBuy = new List<GameObject>();
    private int totalItemsToBuy, placedItems, index,
                numOfPairs, numOfConstrictedItems, numOfFreeItems, numOfItlog;


    private void Start()
    {
        totalItemsToBuy = Random.Range(7,10);

        itemsToBuy = new GameObject[totalItemsToBuy];
        paninda = Resources.LoadAll<GameObject>("Prefabs/ItemPrefabs");     // get all GameObject 

        if (paninda.Length == 0)
        {
            Debug.Log("No assets found at the Prefabs folder!");
            return;
        }

        // populate pair tracker

        for (int i=0; i<7; i++) // only 7 pairs
        {
            pairTracker[i] = false;
        }


        SortPaninda();

        CreateOrder();

        // populate gameobject[]
        for(int i=0; i<itemsToBuy.Length; i++)
        {
            itemsToBuy[i] = listToBuy[i];
        }
        

        

        // while (!isSolvable)
        // {
        //     CreateOrder();
        //     SortOrder();
        //     VerifyOrder();

        //     ResetCounts();

        // }

        foreach(GameObject item in listToBuy)
        {
            Debug.Log(item.name);
        }
    }
    // public void CheckInventory()
    // {
    //     paninda = Resources.LoadAll("Prefabs/ItemPrefabs");

    //     foreach(var item in paninda)
    //     {
    //         Debug.Log(item.name);
    //     }
    // }


    // in a pair, 1 is of type PartOfPair and the other is Free
    // items of type PartOfPair will only appear in the order if the pair is active
    // 'Free' items can appear more than once (only if the pair it belongs to isn't active)
    // if the pair is active, both can't appear again
    private void SortPaninda()
    {
        foreach(GameObject item in paninda)
        {
            if (item.GetComponent<Item>().GetItemType() == ItemType.PartOfPair)
            {
                itemsInPair.Add(item);
            }
            else if (item.GetComponent<Item>().GetItemType() == ItemType.Constricted)
            {
                constrictedItems.Add(item);
            }
            else
            {
                freeItems.Add(item);
            }
        }
    }

    private void CreateOrder()
    {
        // totalItemsToBuy = numOfItems;

        numOfPairs = Random.Range(1,4);             // set based on user level
        numOfConstrictedItems = Random.Range(1,3);  // set based on user level
        numOfFreeItems = totalItemsToBuy - (numOfPairs * 2) - numOfConstrictedItems;

        GeneratePairs();
        GenerateSingleItems();
    }

    // for pair
    private void GeneratePairs()
    {
        for (int i=0; i<numOfPairs; i++)
        {
            int pairID = Random.Range(0,7);

            if (pairTracker[pairID])
            {
                i--;    // pair is already active, choose a diff one
            }
            else
            {
                listToBuy.AddRange(GetPair(pairID));   
            }
        }
    }

    // for constricted and free
    private void GenerateSingleItems()
    {
        int index;

        // constricted
        for (int i=0; i<numOfConstrictedItems; i++)
        {
            index = Random.Range(0, constrictedItems.Count); 
            if (constrictedItems[index].name == "Itlog")
            {
                if (numOfItlog < itlogLimit)
                {
                    listToBuy.Add(constrictedItems[index]);
                    numOfItlog++;
                }
                else
                {
                    i--;
                }
            }
            else
            {
                listToBuy.Add(constrictedItems[index]);
            }

        }

        // free items
        for (int i=0; i<numOfFreeItems; i++)
        {
            index = Random.Range(0, freeItems.Count);
            listToBuy.Add(freeItems[index]);
        }
    }

    // if pair is active, the 'Free' item in the pair is removed from the free list.
    // this means both items in the pair will only appear once.
    private GameObject[] GetPair(int pairID)
    {
        // pair 1 -> ItemType.PartOfPair - will be checked for adjacency rules
        // pair 0 -> ItemType.Free
        GameObject[] pair = new GameObject[2];
        switch (pairID)
        {
            case 0:
                pair[0] = LookForItemWithName("Bawang");
                pair[1] = LookForItemWithName("Sibuyas");

                freeItems.Remove(freeItems.Find(item => item.name == "Bawang"));
                break;
            
            case 1:
                pair[0] = LookForItemWithName("Toyo");
                pair[1] = LookForItemWithName("Suka");
                freeItems.Remove(freeItems.Find(item => item.name == "Toyo"));
                break;

            case 2:
                pair[0] = LookForItemWithName("CornedBeef");
                pair[1] = LookForItemWithName("Patatas");
                freeItems.Remove(freeItems.Find(item => item.name == "CornedBeef"));
                break;

            case 3:
                pair[0] = LookForItemWithName("MeatLoaf");
                pair[1] = LookForItemWithName("Ketchup");
                freeItems.Remove(freeItems.Find(item => item.name == "MeatLoaf"));
                break;

            case 4:
                pair[0] = LookForItemWithTag("Chips");
                pair[1] = LookForItemWithTag("Alcohol");

                // add the specific chips item as matching object
        
                freeItems.RemoveAll(item => item.CompareTag("Chips"));
                break;
            
            case 5:
                pair[0] = LookForItemWithTag("Conditioner");
                pair[1] = LookForItemWithTag("Shampoo");

                
                freeItems.RemoveAll(item => item.CompareTag("Conditioner"));
                break;

            case 6:
                pair[0] = LookForItemWithTag("Biscuit");
                pair[1] = LookForItemWithTag("Soda");

                
                freeItems.RemoveAll(item => item.CompareTag("Biscuit"));
                break;

        }

        GameObject actualItemPair0 = GetActualItemWithName(pair[0].name);
        GameObject actualItemPair1 = GetActualItemWithName(pair[1].name);

        // add to actual prefab instance
        actualItemPair1.GetComponent<Item>().AddMatchingObject(actualItemPair0);

        // pair[1].GetComponent<Item>().AddMatchingObject(pair[0]);

        pairTracker[pairID] = true;
        return pair;
    }

    private GameObject GetActualItemWithName(string itemName)   // actual prefab
    {
        GameObject actualItem = null;
        foreach(GameObject item in paninda)
        {
            if (item.name == itemName)
            {
                actualItem = item;
            }
        }
        return actualItem;
    }

    private GameObject LookForItemWithName(string itemName)
    {
        GameObject foundItem;
        foreach(GameObject item in paninda)
        {
            if (item.name == itemName)
            {
                foundItem = Instantiate(item);
                foundItem.name = foundItem.name.Replace("(Clone)", "");
                return foundItem;
            }
        }
        return null;
    }

    private GameObject LookForItemWithTag(string tag)
    {
        int index;
        GameObject foundItem;
        List<GameObject> itemsWithTag = new List<GameObject>();

        foreach(GameObject item in paninda)
        {
            if (item.CompareTag(tag))
            {
                itemsWithTag.Add(item);
            }
        }

        index = Random.Range(0, itemsWithTag.Count);
        foundItem = Instantiate(itemsWithTag[index]);
        foundItem.name = foundItem.name.Replace("(Clone)", "");

        return foundItem;

    }

    // backtracking algo
    // checks if the generated order is solvable
    private void VerifyOrder()
    {
        // base case - all items have been placed
        if (placedItems == itemsToBuy.Length)
        {
            isSolvable = true;
            return;
        }

        GameObject itemToPlace = itemsToBuy[index];

        for (int i=0; i<possibleSolution.Length; i++)
        {
            if (Placeable(itemToPlace, i))
            {
                possibleSolution[i] = itemToPlace;
                placedItems++;
                index++;

                VerifyOrder();

                if (isSolvable)
                {
                    return;
                }

                // go back if not item is not placeable in slot
                index--;
                placedItems--;
                possibleSolution[i] = null;
            }
        }
    }

    // check if item can be placed in a slot
    private bool Placeable(GameObject itemToPlace, int slotIndex)
    {
        if (possibleSolution[slotIndex] != null) return false;  // slot is already occupied

        // Part of pair 
        if (itemToPlace.GetComponent<Item>().GetItemType() == ItemType.PartOfPair)
        {
            
        }
        return false;
    }

    public void ResetItemMatchings()
    {
        foreach(GameObject item in paninda)
        {
            item.GetComponent<Item>().ResetMatchingObjects();
        }
    }

    // // check if item can be placed in a slot
    // private bool Placeable(GameObject itemToPlace, int slotIndex)   
    // {
    //     if (possibleSolution[slotIndex] != null) return false;  // slot already has an item

    //     // itlog, bawang, toyo, soda
    //     if (itemToPlace.GetComponent<Item>().GetItemType() == ItemType.SlotDecider)
    //     {
    //         if (itemToPlace.name == "Itlog")
    //         {   
    //             // bawal itlog sa corner
    //             if (cannotPlaceItlog.Contains(slotIndex)) 
    //             {
    //                 return false;
    //             }

    //             // bawal bote sa tabi ng itlog
    //             foreach(GameObject neighboringItem in GetNeighboringItems(slotIndex))
    //             {
    //                 if(neighboringItem.CompareTag("Glass Bottle"))
    //                 {
    //                     return false;
    //                 }
    //             }
    //         }

    //         // glass bottle can't be put next to another glass bottle
    //         else if (itemToPlace.CompareTag("Glass Bottle"))
    //         {
    //             foreach(GameObject neighboringItem in GetNeighboringItems(slotIndex))
    //             {
    //                 if(neighboringItem.CompareTag("Glass Bottle"))
    //                 {
    //                     return false;
    //                 }
    //             }
                
    //         }

    //         return true;    // can be placed anywhere
    //     }

    //     // noodles, sibuyas, suka, chips
    //     else if (itemToPlace.GetComponent<Item>().GetItemType() == ItemType.SlotDependent)
    //     {

    //         if (itemToPlace.name == "LuckyMeChicken")
    //         {
    //             if (ilangChickenNoodlesMatched < numOfItlogNoodlesPairs)
    //             {
    //                 if (CheckNeighborPair(itemToPlace, slotIndex))
    //                 {
    //                     ilangChickenNoodlesMatched++;
    //                     return true;
    //                 }
    //                 return false;
    //             }
    //         }

    //         else if (itemToPlace.name == "Sibuyas")
    //         {
    //             if (ilangSibuyasMatched < numOfBawangSibuyasPairs)
    //             {
    //                 if (CheckNeighborPair(itemToPlace, slotIndex))
    //                 {
    //                     ilangSibuyasMatched++;
    //                     return true;
    //                 }
    //                 return false;
    //             }
    //         }

    //         else if (itemToPlace.name == "Suka")
    //         {
    //             if (ilangSukaMatched < numOfToyoSukaPairs)
    //             {
    //                 if (CheckNeighborPair(itemToPlace, slotIndex))
    //                 {
    //                     ilangSukaMatched++;
    //                     return true;
    //                 }
    //                 return false;
    //             }
    //         }

    //         else if (itemToPlace.CompareTag("Chips"))
    //         {
    //             if (ilangChipsMatched < numOfSodaChipsPairs)
    //             {
    //                 if (CheckNeighborPair(itemToPlace, slotIndex))
    //                 {

    //                     ilangChipsMatched++;
    //                     return true;
    //                 }
    //                 return false;
    //             }
    //         }

    //         return true;

    //     }

    //     // can be placed anywhere - to be placed last
    //     else
    //     {
    //         return true;
    //     }
    // }

        // // backtrack backtrack backtrack backtrack
    // private void VerifyOrder()
    // {
    //     // base case - all items have been placed
    //     if (placedItems == itemsToBuy.Length)       // base case
    //     {
    //         isSolvable = true;
    //         return;
    //     }
    //     // --------

    //     GameObject itemToPlace = sortedItemsToBuy[index];

    //     for (int i=0; i<9; i++)     // try to place within the nine slots
    //     {
    //         matchedItem = null;
    //         if(Placeable(itemToPlace, i))
    //         {
    //             possibleSolution[i] = itemToPlace;
    //             placedItems++;
    //             index++;

    //             GameObject currentMatchedItem = matchedItem;

    //             VerifyOrder();  // move on

    //             if (isSolvable)
    //             {
    //                 return;
    //             }
                
    //             // if the current slot doesn't work for the item, backtrack
    //             index--;
    //             placedItems--;
    //             possibleSolution[i] = null;
                
    //             if (currentMatchedItem != null) 
    //             {
    //                 currentMatchedItem.GetComponent<Item>().Unmatch();
                    
    //                 if (itemToPlace.name == "LuckyMeChicken") ilangChickenNoodlesMatched--;
    //                 else if (itemToPlace.name == "Sibuyas") ilangSibuyasMatched--;
    //                 else if (itemToPlace.name == "Suka") ilangSukaMatched--;
    //                 else if (itemToPlace.CompareTag("Chips")) ilangChipsMatched--;
    //             }


    //         }
    //     }

    // }

    // // checks if an item's pair is in the current slot's neighbors
    // private bool CheckNeighborPair(GameObject itemToPlace, int slotIndex)
    // {
    //     List<string> matchingItems = itemToPlace.GetComponent<Item>().GetMatchingItemNames();
    //     List<GameObject> neighbors = GetNeighboringItems(slotIndex);

    //     foreach(GameObject neighbor in neighbors)
    //     {
    //         if (matchingItems.Contains(neighbor.name))
    //         {
    //             Item item = neighbor.GetComponent<Item>();
    //             if (!item.isMatched)
    //             {
    //                 item.Match();
    //                 matchedItem = neighbor;
    //                 return true;    
    //             }
    //         }
    //     }
    //     return false;
    // }

    // private List<GameObject> GetNeighboringItems(int slotIndex)
    // {
    //     List<GameObject> neighboringItems = new List<GameObject>();
    //     // List<int> neighborSlots = new List<int>();

    //     int row = slotIndex / 3;
    //     int col = slotIndex % 3;
        
    //     if (row>0 && possibleSolution[slotIndex-3] != null)
    //     {
    //         neighboringItems.Add(possibleSolution[slotIndex-3]);
    //     }
        
    //     if (row<2 && possibleSolution[slotIndex+3] != null)
    //     {
    //         neighboringItems.Add(possibleSolution[slotIndex+3]);
    //     }

    //     if (col>0 && possibleSolution[slotIndex-1] != null) 
    //     {
    //         neighboringItems.Add(possibleSolution[slotIndex-1]);
    //     }

    //     if (col<2 && possibleSolution[slotIndex+1] != null) 
    //     {
    //         neighboringItems.Add(possibleSolution[slotIndex+1]);
    //     }

    //     return neighboringItems;
    // }

    // private void GetNumOfPairs()
    // {
    //     CountItemsWithMatchingRule();

    //     numOfItlogNoodlesPairs = Mathf.Min(numOfItlog, numOfChickenNoodles);
    //     numOfBawangSibuyasPairs = Mathf.Min(numOfBawang, numOfSibuyas);
    //     numOfSodaChipsPairs = Mathf.Min(numOfSoda, numOfChips);
    //     numOfToyoSukaPairs = Mathf.Min(numOfToyo, numOfSuka);
    // }

    // private GameObject LookFor(string name, string keyType, List<GameObject> items)
    // {
    //     if (keyType == "Name")
    //     {
    //         foreach(GameObject item in items)
    //         {
    //             if (item.name == name)
    //             {
    //                 return item;
    //             }
    //         }
    //     }

    //     else if (keyType == "Tag")
    //     {
    //         foreach(GameObject item in items)
    //         {
    //             if (item.CompareTag(name))
    //             {
    //                 return item;
    //             }
    //         }
    //     }

    //     return null;
    // }

    // // put matching items first
    // private void SortOrder()
    // {
    //     sortedItemsToBuy.Clear();

    //     // create a temporary list (final state should be the non-matched items)
    //     List<GameObject> temp = new List<GameObject>();

    //     foreach (GameObject item in itemsToBuy)
    //     {
    //         temp.Add(item);
    //     }

    //     GetNumOfPairs();

    //     if (numOfItlogNoodlesPairs > 0)
    //     {
    //         for (int i=0; i<numOfItlogNoodlesPairs; i++)
    //         {
    //             GameObject itlog = LookFor("Itlog", "Name", temp);
    //             GameObject noodles = LookFor("LuckyMeChicken", "Name", temp);

    //             sortedItemsToBuy.Add(itlog);
    //             sortedItemsToBuy.Add(noodles);

    //             temp.Remove(itlog);
    //             temp.Remove(noodles);
    //         }
    //     }

    //     if (numOfBawangSibuyasPairs > 0)
    //     {
    //         for (int i=0; i<numOfBawangSibuyasPairs; i++)
    //         {
    //             GameObject bawang = LookFor("Bawang", "Name", temp);
    //             GameObject sibuyas = LookFor("Sibuyas", "Name", temp);

    //             sortedItemsToBuy.Add(bawang);
    //             sortedItemsToBuy.Add(sibuyas);

    //             temp.Remove(bawang);
    //             temp.Remove(sibuyas);
    //         }
    //     }

    //     if (numOfToyoSukaPairs > 0)
    //     {
    //         for (int i=0; i<numOfToyoSukaPairs; i++)
    //         {
    //             GameObject toyo = LookFor("Toyo", "Name", temp);
    //             GameObject suka = LookFor("Suka", "Name", temp);

    //             sortedItemsToBuy.Add(toyo);
    //             sortedItemsToBuy.Add(suka);

    //             temp.Remove(toyo);
    //             temp.Remove(suka);
    //         }
    //     }

    //     if (numOfSodaChipsPairs > 0)
    //     {
    //         for (int i=0; i<numOfSodaChipsPairs; i++)
    //         {
    //             GameObject soda = LookFor("Soda", "Tag", temp);
    //             GameObject chips = LookFor("Chips", "Tag", temp);

    //             sortedItemsToBuy.Add(soda);
    //             sortedItemsToBuy.Add(chips);

    //             temp.Remove(soda);
    //             temp.Remove(chips);
    //         }
    //     }

    //     sortedItemsToBuy.AddRange(temp);  // add the rest of the items to the sorted list

    // }

    // private void CountItemsWithMatchingRule()
    // {
    //     foreach (GameObject item in itemsToBuy)
    //     {
    //         if (item.name == "LuckyMeChicken") numOfChickenNoodles++;

    //         if (item.name == "Bawang") numOfBawang++;

    //         if (item.name == "Sibuyas") numOfSibuyas++;

    //         if (item.name == "Toyo") numOfToyo++;

    //         if (item.name == "Suka") numOfSuka++;

    //         if (item.CompareTag("Soda")) numOfSoda++;

    //         if (item.CompareTag("Chips")) numOfChips++;
            
    //     }
    // }

    // // private void ResetMatches()
    // // {
    // //     foreach (GameObject item in possibleSolution)
    // //     {
    // //         if (item != null)
    // //         {
    // //             item.GetComponent<Item>().Unmatch();
    // //         }
    // //     }
    // // }

    // private void ResetCounts()
    // {
    //     numOfItlog = numOfBawang = numOfSoda = numOfToyo = 0;
    //     numOfChickenNoodles = numOfSibuyas = numOfChips = numOfSuka = 0;
    //     ilangChickenNoodlesMatched = ilangSibuyasMatched = ilangSukaMatched = ilangChipsMatched = 0;
    //     numOfItlogNoodlesPairs = numOfBawangSibuyasPairs = numOfToyoSukaPairs = numOfSodaChipsPairs = 0;
    // }

    public GameObject[] GetItems()
    {
        return itemsToBuy;
    }

    
    
}