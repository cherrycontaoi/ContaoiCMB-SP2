using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{

    [SerializeField] Order orderPrefab;
    [SerializeField] private Sprite[] expressions;
    [SerializeField] private string customerName;
    private SpriteRenderer customerSpriteRenderer;
    private CustomerState customerState = CustomerState.Idle;
    [SerializeField] public Order order;
    public bool isDone;

    // Dialogue stuff
    // public List<Dialogue> dialogueData;
    private WaypointMover waypointMover;
    private Coroutine typingCoroutine;
    private Coroutine talkingCoroutine;
    private DialogueController dialogueController;
    public Dialogue dialogueData;       // dialogue to be displayed.
    public Dialogue smallTalkDialogue;
    public Dialogue orderDialogue;
    public Dialogue endingDialogue;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive, isEndingDialogue;

    // hasSmallTalk -> hindi bibili agad, willBuy -> bibili (generate order)
    public bool hasSmallTalk, willBuy, willBuyAfterSmallTalk, currentlyBuying;       

    public Dictionary<string, bool> orderErrors = new Dictionary<string, bool>()
    {
        { "wrongPlasticBag", false },
        { "noMatchingItem", false },            // matching items not adjacent
        { "itlogNotLast", false },              // itlog not placed last
        { "cannedNotFirst", false },            // canned not placed first
        { "itlogAndBottle", false },            // itlog next to bottle
        { "itlogAtCorner", false },             // itlog at the corner
        { "bottleAndBottle" , false },          // bottle next to bottle
        { "detergentAndProduce", false },       // detergent next to produce
        { "ajinomotoAndMagicSarap", false },    // ajinomoto next to magic sarap
        { "notSameCount", false },              // items given != order
    };

    void Start()
    {
        waypointMover = GetComponent<WaypointMover>();
    }

    void Awake()
    {
        dialogueController = DialogueController.Instance;
    }

    public void ShowCustomer()
    {
        // position ng orderan, show customer here
        // transform.position = new Vector3(-5f, 3.61f, 1f);
        
        // initial state
        customerSpriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(gameObject.name + "'s current state is: " + customerState);
        ChangeCustomerState(CustomerState.Idle);
    }

    public void StartCustomer()
    {

        ShowCustomer();

        if (willBuy)
        {
            order = Instantiate(orderPrefab, transform);
            order.GenerateOrder();      // create order;

            orderDialogue = DialogueBuilder.BuildOrderDialogue(order, hasSmallTalk);
            // SaveNewDialogue(orderDialogue);        // for tracking purposes maybe, jic

            if (hasSmallTalk)
            {
                // small talk first
                dialogueData = smallTalkDialogue;
                willBuyAfterSmallTalk = true;
                // then order
            }
            else    // will buy only
            {
                // set order as the dialogue right away.
                currentlyBuying = true;
                dialogueData = orderDialogue;
            }
        }
        else        // small talk only
        {
            dialogueData = smallTalkDialogue;
        }
        if (talkingCoroutine == null)
        {
            talkingCoroutine = StartCoroutine(StartTalking());
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {   
        // no dialogue or menu is open
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;
        }

        if (isDialogueActive)
        {
            // next line in dialogue
            NextLine();
        }
        else
        {
            // start dialogue , doesn't really get called (clean up later!!!)
            StartDialogue();
        }
    }

    private IEnumerator StartTalking()
    {
        // delay 2 secs
        yield return new WaitForSeconds(2f);

        talkingCoroutine = null;

        if (dialogueData.dialogueType == DialogueType.Buying || dialogueData.dialogueType == DialogueType.SmallTalk)
        {
            ChangeCustomerState(CustomerState.Talking);
        }
        else
        {
            ChangeCustomerState(CustomerState.CheckingItems);
        }
        // already active with the first line in the dialogue;
        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueController.ShowDialogueUI(true);
        DisplayCurrentLine();
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialogueController.ShowDialogueUI(true);
        // PauseController.SetPause(false);     //true - prevents things from happening

        //type line
        DisplayCurrentLine();
    }

    private void NextLine()
    {
        if (isTyping) 
        {
            // skip the typing animation and show the full line
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueController.SetDialogueText(dialogueData.lines[dialogueIndex]);
            isTyping = false;
            return;
        }

        // clear choices
        dialogueController.ClearChoices();

        // check endDialogueLines
        if(dialogueData.endLines != null && dialogueData.endLines.Count > dialogueIndex && dialogueData.endLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        if(dialogueData.dialogueType == DialogueType.SmallTalk)
        {
            // check if there are choices and display
            foreach(DialogueChoice dialogueChoice in dialogueData.choices)
            {
                if(dialogueChoice.dialogueIndex == dialogueIndex)
                {
                    // display choices
                    DisplayChoices(dialogueChoice);
                    return;
                }
            }
        }

        if (++dialogueIndex < dialogueData.lines.Count)  // still have a line
        {
            DisplayCurrentLine(); // type next line
        }
        else
        {
            // end the dialogue - no more lines
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueController.SetDialogueText("");

        foreach(char letter in dialogueData.lines[dialogueIndex])
        {
            dialogueController.SetDialogueText(dialogueController.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        if (dialogueData.linesToAutoProgress.Count > dialogueIndex && dialogueData.linesToAutoProgress[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            isTyping = false;
            typingCoroutine = null;
            // display next line
            NextLine();
        }
        else
        {
            isTyping = false;
            typingCoroutine = null;
        }
        
    }

    void DisplayChoices(DialogueChoice choice)
    {
        ChangeCustomerState(CustomerState.Idle);
        for(int i=0; i<choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndices[i];
            dialogueController.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueController.ClearChoices();
        ChangeCustomerState(CustomerState.Talking);
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // either neutral or happy expression at the end
        if (dialogueIndex == dialogueData.lines.Count - 1 && dialogueData.dialogueType == DialogueType.EndDialogue)
        {
            ChangeCustomerState(TransactionController.Instance.GetFinalExpression());
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        dialogueController.SetDialogueText("");
        dialogueController.ShowDialogueUI(false);

        if (isEndingDialogue)
        {
            CounterDisplayController.Instance.DisplayMoney();
            isEndingDialogue = false;
            dialogueData = null;
            isDone = true;
            waypointMover.isBuying = false;
            
            if (orderDialogue != null && endingDialogue != null)
                {
                    SaveNewDialogue(orderDialogue);
                    SaveNewDialogue(endingDialogue);
            }

            return;
        }

        if (willBuyAfterSmallTalk)
        {
            willBuyAfterSmallTalk = false;

            // buying time
            dialogueData = orderDialogue;
            currentlyBuying = true;
            // StartTalking();
            if (talkingCoroutine == null)
            {
                talkingCoroutine = StartCoroutine(StartTalking());
            }
        }
        else
        {
            // everything done
            dialogueData = null;

            if (!willBuy)   // only small talk. end already
            {
                ChangeCustomerState(CustomerState.Idle);
                isDone = true;
                waypointMover.isBuying = false;
            }
        }
    }

    public void StartEndingDialogue()
    {
        endingDialogue = DialogueBuilder.BuildEndDialogue(orderErrors);    // dialogue for thank u etc.
        // SaveNewDialogue(endingDialogue);   

        isEndingDialogue = true;
        dialogueData = endingDialogue;
        ChangeCustomerState(CustomerState.CheckingItems);
        if (talkingCoroutine == null)
        {
            talkingCoroutine = StartCoroutine(StartTalking());
        }
        // StartTalking();
    }


    // private void DestroyNPC()
    // {
    //     // if (orderDialogue != null && endingDialogue != null)
    //     // {
    //     //     SaveNewDialogue(orderDialogue);
    //     //     SaveNewDialogue(endingDialogue);
    //     // }

    //     // add walking away animation
    //     isDone = true;
    //     Destroy(gameObject);
    // }

    private void SaveNewDialogue(Dialogue dialogue)
    {
    
        // for unity_editor onlyyyy
        // string filename;
        // string path = "Assets/Resources/Dialogues/" + customerName + "/";

        // if (dialogue.dialogueType == DialogueType.Buying)
        // {
        //     filename = "Order";
        // }
        // else    // end dialogue
        // {
        //     filename = "EndDialogue";
        // }

        // string filePath = path + filename + ".asset";

        // int orderCount = 1;
        
        // // if there is already an existing file named Order.asset
        // // name the others like this: Order (n).asset
        // while (AssetDatabase.LoadAssetAtPath<Dialogue>(filePath) != null)
        // {
        //     filePath = path + filename + " (" + orderCount + ").asset";
        //     orderCount++;
        // }

        // AssetDatabase.CreateAsset(dialogue, filePath);
        // AssetDatabase.SaveAssets();
    

        string filename;

        if (dialogue.dialogueType == DialogueType.Buying)
        {
            filename = "Order";
        }
        else
        {
            filename = "EndDialogue";
        }

        string directoryPath = Path.Combine(Application.persistentDataPath, "Dialogues", customerName);
        Directory.CreateDirectory(directoryPath);

        string filePath = Path.Combine(directoryPath, filename + ".json");

        int count = 1;

        // if there is already an existing file named Order.asset
        // name the others like this: Order (n).asset
        while (File.Exists(filePath))
        {
            filePath = Path.Combine(directoryPath, filename + " (" + count + ").json");
            count++;
        }

        string json = JsonUtility.ToJson(dialogue, true);
        File.WriteAllText(filePath, json);

    }



    public void ChangeCustomerState(CustomerState newCustomerState)
    {
        customerState = newCustomerState;

        switch (customerState)
        {
            case CustomerState.Idle:
                // Debug.Log("Customer is waiting.");
                customerSpriteRenderer.sprite = expressions[1];     // just smile
                break;

            case CustomerState.Talking:
                Debug.Log("Customer is talking.");
                customerSpriteRenderer.sprite = expressions[0];
                break;
            
            case CustomerState.CheckingItems:
                Debug.Log("Customer is checking what you gave.");
                customerSpriteRenderer.sprite = expressions[3];
                break;
            
            case CustomerState.HappyExpression:
                Debug.Log("Customer is satisfied with the service!");
                customerSpriteRenderer.sprite = expressions[1];
                break;
            
            case CustomerState.NeutralExpression:
                Debug.Log("Customer is not so satisfied.");
                customerSpriteRenderer.sprite = expressions[2];
                break;
        }
    }

}

public enum CustomerState
{
    Idle,
    Talking,
    CheckingItems,
    HappyExpression,
    NeutralExpression
}
