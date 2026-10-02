using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsToBuy;
    private GameObject[] possibleSolution = new GameObject[9];      // nine slots
    
    private List<int> cornerSlots = new List<int>{0,2,6,8};    // cant place itlog here

    private int itlogLimit;
    private bool isSolvable;
    private Difficulty difficulty;
    private PlayerData playerData;

    // private int numOfGenerations;

    [SerializeField] private Dictionary<int, bool> pairTracker = new Dictionary<int, bool>(); // tracks which pairs are active
    private GameObject[] paninda;   // all the paninda
    private List<GameObject> itemsInPair = new List<GameObject>();      // itemtype = partofpair
    private List<GameObject> constrictedItems = new List<GameObject>(); // individual items w/ rules
    private List<GameObject> freeItems = new List<GameObject>();        // no rules, can be placed anywhere
    private List<GameObject> listToBuy = new List<GameObject>();
    private int totalItemsToBuy, placedItems, index,
                numOfPairs, numOfConstrictedItems, numOfFreeItems, numOfItlog;
  
    public void GenerateOrder()
    {
        playerData = FindAnyObjectByType<PlayerData>();

        paninda = Resources.LoadAll<GameObject>("Prefabs/ItemPrefabs");     // get all paninda GameObjects 

        if (paninda.Length == 0)
        {
            Debug.Log("No assets found at the Prefabs folder!");
            return;
        }

        while (!isSolvable)
        {
            ResetVars();
            SortPaninda();
            CreateOrder();
            // numOfGenerations++;
            VerifyOrder();
        }
        // Debug.Log("There are "+numOfGenerations+ " order generations");

        totalItemsToBuy = numOfPairs + numOfConstrictedItems + numOfFreeItems;
        itemsToBuy = new GameObject[totalItemsToBuy];

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

        foreach(GameObject item in itemsToBuy)
        {
            Debug.Log(item.name);
        }
    }

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

    private void GetNums(Difficulty difficulty)
    {
        switch (difficulty)
        {
            // 1 pair, 1-2 constricted, 1-5 free
            case Difficulty.Easy:
                numOfPairs = 1;
                numOfConstrictedItems = Random.Range(1,3);
                numOfFreeItems = Random.Range(1,6);
                itlogLimit = 2;
                break;

            // 2 pairs, 2-3 constricted, 1-2 free
            case Difficulty.Medium:
                numOfPairs = 2;
                numOfConstrictedItems = Random.Range(2,4);
                numOfFreeItems = Random.Range(1,3);
                itlogLimit = 3;
                break;

            // 2 pairs, 3-4 constricted, 1-2 free
            case Difficulty.Hard:
                numOfPairs = 2;
                numOfConstrictedItems = Random.Range(3,5);
                numOfFreeItems = Random.Range(1,3);
                itlogLimit = 3;
                break;
        }
    }

    private void CreateOrder()
    {
        difficulty = playerData.GetPlayerPuzzleLevel();
        GetNums(difficulty);
        // numOfPairs = Random.Range(1,4);             // set based on user level
        // numOfConstrictedItems = Random.Range(1,3);  // set based on user level
        // numOfFreeItems = totalItemsToBuy - (numOfPairs * 2) - numOfConstrictedItems;

        GeneratePairs();
        GenerateSingleItems();

        // Structure of the created Order:
        // Pairs -> Free Items -> Constricted Items
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

        // free items
        for (int i=0; i<numOfFreeItems; i++)
        {
            index = Random.Range(0, freeItems.Count);
            listToBuy.Add(freeItems[index]);
        }

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

        // add to actual prefab instance
        pair[1].GetComponent<Item>().AddMatchingObject(pair[0]);

        pairTracker[pairID] = true;
        return pair;
    }

    private GameObject LookForItemWithName(string itemName)
    {
        foreach(GameObject item in paninda)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }

    private GameObject LookForItemWithTag(string tag)
    {
        int index;
        List<GameObject> itemsWithTag = new List<GameObject>();

        foreach(GameObject item in paninda)
        {
            if (item.CompareTag(tag))
            {
                itemsWithTag.Add(item);
            }
        }

        index = Random.Range(0, itemsWithTag.Count);
        return itemsWithTag[index];

    }

    // backtracking algo
    // checks if the generated order is solvable
    private void VerifyOrder()
    {
        // base case - all items have been placed
        if (placedItems == listToBuy.Count)
        {
            isSolvable = true;
            return;
        }

        GameObject itemToPlace = listToBuy[index];

        for (int i=0; i<9; i++)
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
    // ORDER OF PLACING: PartOfPair -> Free Items -> Constricted
    private bool Placeable(GameObject itemToPlace, int slotIndex)
    {
        if (possibleSolution[slotIndex] != null) return false;  // slot is already occupied
        
        // MATCHING ITEMS ------------------------------------------------------------------------
        // items with this type are first to be placed since theyre added to the list first
        // Part of pair 
        if (itemToPlace.GetComponent<Item>().GetItemType() == ItemType.PartOfPair)
        {   
            // GLASS BOTTLE (alcohol, ketchup) ---------------------------------------------------
            // apply rule for Bottle (can't be placed next to another bottle)
            if (itemToPlace.GetComponent<Item>().IsItGlassBottle())
            {
                // get the neighbors of the possible slot
                List<GameObject> neighbors = GetNeighboringItems(slotIndex);

                foreach(GameObject neighbor in neighbors)
                {   
                    // if it's a bottle, can't place there
                    if (neighbor.GetComponent<Item>().IsItGlassBottle())
                    {
                        return false;
                    }
                }
            }

            return true;    // no other PartOfPair items have specific rules
        }

        // INDIVIDUAL ITEMS ---------------------------------------------------------------------

        else if (itemToPlace.GetComponent<Item>().GetItemType() == ItemType.Constricted)
        {
            List<GameObject> neighbors = GetNeighboringItems(slotIndex);

            // Itlog ----------------------------------------------------------------------------
            if (itemToPlace.name == "Itlog"){
                
                // RULE: Itlog cannot be place in a corner slot
                if (cornerSlots.Contains(slotIndex))
                {
                    return false;
                }

                // RULE: Itlog cannot be placed next to glass bottles
                foreach(GameObject neighbor in neighbors)
                {
                    if (neighbor.GetComponent<Item>().IsItGlassBottle())
                    {
                        return false;
                    }
                }
            }    

            // Glass Bottle --------------------------------------------------------------------
            if (itemToPlace.GetComponent<Item>().IsItGlassBottle())
            {
                foreach(GameObject neighbor in neighbors)
                {
                    // RULE: Glass Bottle can't be placed next to another one
                    if (neighbor.GetComponent<Item>().IsItGlassBottle())
                    {
                        return false;
                    }
                }
            }      

            // Detergent -----------------------------------------------------------------------
            if (itemToPlace.CompareTag("Detergent"))
            {
                foreach(GameObject neighbor in neighbors)
                {
                    // RULE: Detergent can't be placed next to Produce
                    if (neighbor.CompareTag("Produce")){
                        return false;
                    }
                }
            }

            // Ajinomoto ----------------------------------------------------------------------
            if (itemToPlace.name == "Ajinomoto")
            {
                foreach(GameObject neighbor in neighbors)
                {
                    // RULE: Ajinomoto can't be placed next to MagicSarap
                    if (neighbor.name == "MagicSarap")
                    {
                        return false;
                    }
                }
            }
        }

        // FREE ITEMS ------------------------------------------------------------------

        return true;
    }

    private void ResetPairTracker()
    {
        // reset pairTracker
        for (int i=0; i<7; i++) // only 7 pairs
        {
            pairTracker[i] = false;
        }
    }
    private void ResetVars()
    {
        // clear all lists
        numOfItlog = 0;
        placedItems = 0;
        index = 0;

        itemsInPair.Clear();
        constrictedItems.Clear();
        freeItems.Clear();
        listToBuy.Clear();

        isSolvable = false;
        
        ResetPairTracker();

        // reset possible solution list
       for(int i=0; i<possibleSolution.Length; i++)
        {
            possibleSolution[i] = null;
        }
    }  

    public void ResetItemMatchings()
    {
        foreach(GameObject item in paninda)
        {
            item.GetComponent<Item>().ResetMatchingObjects();
        }
    }

        private List<GameObject> GetNeighboringItems(int slotIndex)
    {
        List<GameObject> neighboringItems = new List<GameObject>();
        // List<int> neighborSlots = new List<int>();

        int row = slotIndex / 3;
        int col = slotIndex % 3;
        
        if (row>0 && possibleSolution[slotIndex-3] != null)
        {
            neighboringItems.Add(possibleSolution[slotIndex-3]);
        }
        
        if (row<2 && possibleSolution[slotIndex+3] != null)
        {
            neighboringItems.Add(possibleSolution[slotIndex+3]);
        }

        if (col>0 && possibleSolution[slotIndex-1] != null) 
        {
            neighboringItems.Add(possibleSolution[slotIndex-1]);
        }

        if (col<2 && possibleSolution[slotIndex+1] != null) 
        {
            neighboringItems.Add(possibleSolution[slotIndex+1]);
        }

        return neighboringItems;
    }

    public GameObject[] GetItems()
    {
        return itemsToBuy;
    }   
}