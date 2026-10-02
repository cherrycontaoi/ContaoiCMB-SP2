using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerDialogue", menuName = "Customer Dialogue")]

public class Dialogue : ScriptableObject
{
    public List<string> lines = new List<string>();
    public List<bool> linesToAutoProgress = new List<bool>();     // doesn't wait for the player to press a key before moving on to the next dialogue
    public List<bool> endLines = new List<bool>();                // lines where the dialogue ends
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public DialogueType dialogueType;

    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;           // dialogue line with question (choices appear here)
    public string[] choices;             // response option
    public int[] nextDialogueIndices;   // where choice leads
}

public enum DialogueType
{
    SmallTalk,
    Buying,
    EndDialogue
}