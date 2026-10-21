using System.Collections.Generic;
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

    public static string[] errorFeedbackLines = new string[]
    {
        "Nasaan yung kasama ng...",
        "Ah! Nandito pala.",

        "Baka mabasag yung itlog.",

        "Akala ko nasa ilalim yung de lata.",

        "Hala, baka madaganan ng bote yung itlog.",

        "Nasa bungad yung itlog? Sana hindi mabasag...",

        "Sana hindi mabasag yung bote.",
        
        "Baka mabutas yung sabong pang-laba ah.",

        "Ay, dalawang seasoning pala ang nasabi ko.",
        
        "Kumpleto naman yung laman ng plastic bag, diba?",

        "Ay, akin 'to?"
    };

    public static string[] perfectScoreResponses = new string[]
    {
        "Uy, salamat!",
        "Ganyan talaga 'pag bata pa, hindi pa madaling makalimot.",
        "Sana ikaw lagi 'yung nakabantay sa tindahan."
    };

    public static string[] notPerfectScoreResponses = new string[]
    {
        "'Sya, salamat ha.",
        "Sige...",
        "Babalik nalang ako kapag may kulang, ha?"
    };

    public static Dialogue BuildOrderDialogue(Order order, bool hasSmallTalk)
    {
        GameObject[] customerOrder = order.GetItems();
        // Debug.Log("Customer order length: " + customerOrder.Length);
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
            if (i == numOfLines-1)  // automatically
            {
                orderDialogue.linesToAutoProgress.Add(true);
            }
            else
            {
                orderDialogue.linesToAutoProgress.Add(false);    
            }
            
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

    public static Dialogue BuildEndDialogue(Dictionary<string, bool> errors)
    {
        Dialogue endDialogue = ScriptableObject.CreateInstance<Dialogue>();

        // foreach (KeyValuePair<string, bool> error in errors)
        // {
        //     Debug.Log(error.Key + " = " + error.Value);
        // }

        // Fill lines
        foreach(string errorKey in errors.Keys)
        {

            if (!errors[errorKey]) continue;    // if value is false

            switch (errorKey)
            {
                case "noMatchingItem":
                    endDialogue.lines.Add(errorFeedbackLines[0]);
                    endDialogue.lines.Add(errorFeedbackLines[1]);
                    break;

                case "itlogNotLast":
                    endDialogue.lines.Add(errorFeedbackLines[2]);
                    break;
                
                case "cannedNotFirst":
                    endDialogue.lines.Add(errorFeedbackLines[3]);
                    break;

                case "itlogAndBottle":
                    endDialogue.lines.Add(errorFeedbackLines[4]);
                    break;

                case "itlogAtCorner":
                    endDialogue.lines.Add(errorFeedbackLines[5]);
                    break;
                
                case "bottleAndBottle":
                    endDialogue.lines.Add(errorFeedbackLines[6]);
                    break;
                
                case "detergentAndProduce":
                    endDialogue.lines.Add(errorFeedbackLines[7]);
                    break;

                case "ajinomotoAndMagicSarap":
                    endDialogue.lines.Add(errorFeedbackLines[8]);
                    break;

                case "notSameCount":
                    endDialogue.lines.Add(errorFeedbackLines[9]);
                    break;

                case "wrongPlasticBag":
                    endDialogue.lines.Add(errorFeedbackLines[10]);
                    break;
            }

            endDialogue.lines.Add("...");       // transition
                
        }

        // thank u line
        int index = Random.Range(0, 3);     // 3 options perfect/not perfect
        if (errors.ContainsValue(true)) // has at least 1 mistake
        {
            endDialogue.lines.Add(notPerfectScoreResponses[index]);
        }
        else
        {
            endDialogue.lines.Add(perfectScoreResponses[index]);
        }


        int numOfLines = endDialogue.lines.Count;

        // FILL linesToAutoProgress -----------------------------------------------
        for(int i=0; i<numOfLines; i++)
        {
            if(i == numOfLines - 1) // last element
            {
                endDialogue.linesToAutoProgress.Add(true);  // automatically end last line
            }
            else
            {
                endDialogue.linesToAutoProgress.Add(false);   
            }
        }

        // FILL endLines ----------------------------------------------------------
        for(int i=0; i<numOfLines; i++)
        {
            if(i == numOfLines - 1) // last element
            {
                endDialogue.endLines.Add(true);
            }
            else
            {
                endDialogue.endLines.Add(false);
            }
        }

        // INITIALIZE DialogueType ------------------------------------------------
        endDialogue.dialogueType = DialogueType.EndDialogue;



        return endDialogue;
    }

    
}
