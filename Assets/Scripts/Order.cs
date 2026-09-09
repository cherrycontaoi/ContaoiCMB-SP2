using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private Item[] itemsToBuy;
    private Item[] paninda;
    private int numOfItems;       // max num of items is 9

    private void Start()
    {
        numOfItems = Random.Range(1,9);
        itemsToBuy = new Item[numOfItems];
        paninda = Resources.LoadAll<Item>("Prefabs/ItemPrefabs");     // get all Item 

        if (paninda.Length == 0)
        {
            Debug.Log("No assets found at the Prefabs folder!");
            return;
        }

        CreateOrder();

        foreach(Item item in itemsToBuy)
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

    private void CreateOrder()
    {
        for(int i=0; i<numOfItems; i++)
        {
            int index = Random.Range(0, paninda.Length);
            itemsToBuy[i] = paninda[index];     // randomize ano bibilhin
        }
    }
    
}
