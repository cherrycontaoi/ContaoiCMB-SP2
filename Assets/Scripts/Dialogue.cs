using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerDialogue", menuName = "Customer Dialogue")]

public class Dialogue : ScriptableObject
{
    public string[] lines;
    public bool [] linesToAutoProgress;     // doesn't wait for the player to press a key before moving on to the next dialogue
    public bool [] endLines;                // lines where the dialogue ends
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public DialogueType dialogueType;

}

public enum DialogueType
{
    SmallTalk,
    Buying
}