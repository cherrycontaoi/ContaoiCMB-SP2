using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialogueBuilder : MonoBehaviour
{
    public static Dictionary<int, string> quantityInWords = new Dictionary<int, string>()
    {
        { 1, "Isang" },
        { 2, "Dalawang" },
        { 3, "Tatlong" },
        { 4, "Apat na" },
        { 5, "Limang" },
    };

    public static string[] transitionLines = new string[]  // transition between small talk and buying
    {
        "Ay, ito pala, pabili nga ako.",
        "'Sya, pabili ako.",
        "Ano nga ba 'yun... Ah, pabili ako."
    };

    // public static string[] starterLines = new string[] // customer is directly buying (no ST)
    // {
    //     "Pabili po.",
    //     "Hello, pabili ako."
    // };
    public static Dialogue BuildOrderDialogue(Order order, bool hasSmallTalk)
    {
        GameObject[] customerOrder = order.GetItems();
        Debug.Log("Customer order length: " + customerOrder.Length);
        Dictionary<string, int> itemsToBuy = new Dictionary<string, int>();
        Dialogue orderDialogue = ScriptableObject.CreateInstance<Dialogue>();
        
        foreach (GameObject item in customerOrder)
        {
            string keyName = item.GetComponent<Item>().displayName + item.GetComponent<Item>().supplementaryNameInfo;

            if (itemsToBuy.ContainsKey(keyName))  // already in the dictionary, quantity > 1
            {
                itemsToBuy[keyName]++;
            }
            else
            {
                itemsToBuy.Add(keyName, 1);
            }
        }

        if (hasSmallTalk)
        {
            int index = Random.Range(0, transitionLines.Length);
            orderDialogue.lines.Add(transitionLines[index]);
        }
        else
        {
            // starter line, no small talk
            orderDialogue.lines.Add("Pabili po.");
        }

        string lineToAdd;
        string quantity;
        int currentItemIndex = 0;  // for tracking the last item in itemsToBuy

        // FILL lines --------------------------------------------------------------
        // like this: Isang Coke, Isang Sibuyas, tapos isang Bawang.
        foreach(var item in itemsToBuy)
        {
            quantity = quantityInWords[item.Value];
            
            if (currentItemIndex == itemsToBuy.Count - 1) // last item
            {
                lineToAdd = "tapos " + quantity + " " + item.Key + ".";
            }
            else        // not last item
            {
                lineToAdd = quantity + " " + item.Key + ",";
            }

            currentItemIndex++;

            // Debug.Log("Line to Add: "+ lineToAdd);
            orderDialogue.lines.Add(lineToAdd);
        }

        int numOfLines = orderDialogue.lines.Count;

        // FILL linesToAutoProgress -----------------------------------------------
        for(int i=0; i<numOfLines; i++)
        {
            orderDialogue.linesToAutoProgress.Add(false);
        }

        // FILL endLines ----------------------------------------------------------
        for(int i=0; i<numOfLines; i++)
        {
            if(i == numOfLines - 1) // last element
            {
                orderDialogue.endLines.Add(true);
            }
            else
            {
                orderDialogue.endLines.Add(false);
            }
        }

        // INITIALIZE DialogueType ------------------------------------------------
        orderDialogue.dialogueType = DialogueType.Buying;

    return orderDialogue;
         


    }
    
}
