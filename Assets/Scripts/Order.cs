using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private GameObject[] itemsToBuy;
    private GameObject[] paninda;
    private int numOfItems;       // max num of items is 9

    private void Start()
    {
        numOfItems = Random.Range(1,9);
        itemsToBuy = new GameObject[numOfItems];
        paninda = Resources.LoadAll<GameObject>("Prefabs/ItemPrefabs");     // get all GameObject 

        if (paninda.Length == 0)
        {
            Debug.Log("No assets found at the Prefabs folder!");
            return;
        }

        CreateOrder();

        foreach(GameObject item in itemsToBuy)
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

    public GameObject[] GetItems()
    {
        return itemsToBuy;
    }
    
}
