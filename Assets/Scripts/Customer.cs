using System.Collections;
using System.Collections.Generic;
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
    private DialogueController dialogueController;
    public Dialogue dialogueData;       // dialogue to be displayed.
    public Dialogue smallTalkDialogue;
    public Dialogue orderDialogue;
    public Dialogue endDialogue;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // hasSmallTalk -> hindi bibili agad, willBuy -> bibili (generate order)
    public bool hasSmallTalk, willBuy, willBuyAfterSmallTalk;       

    void Awake()
    {
        dialogueController = DialogueController.Instance;
    }

    public void ShowCustomer()
    {
        // position ng orderan, show customer here
        transform.position = new Vector3(-5f, 3.61f, 1f);
        
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
            SaveNewOrderDialogue(orderDialogue);        // for tracking purposes maybe, jic

            if (hasSmallTalk)
            {
                // small talk first
                dialogueData = smallTalkDialogue;
                willBuyAfterSmallTalk = true;
                StartCoroutine(StartTalking());
                // // then order
                // dialogueData = orderDialogue;
                // StartCoroutine(StartTalking());
            }
            else    // will buy only
            {
                // set order as the dialogue right away.
                dialogueData = orderDialogue;
                StartCoroutine(StartTalking());
            }
        }
        else        // small talk only
        {
            dialogueData = smallTalkDialogue;
            // dialogueData = smallTalkDialogue
            StartCoroutine(StartTalking());
        }

    }

    // private IEnumerator SmallTalkThenBuy()
    // {
    //     // start small talk first
    //     dialogueData = smallTalkDialogue;
    //     yield return StartCoroutine(StartTalking());

    //     // still making small talk
    //     while (isDialogueActive)
    //     {
    //         yield return null;
    //     }

    //     dialogueData = orderDialogue;
    //     yield return StartCoroutine(StartTalking());
    // }


    private void SaveNewOrderDialogue(Dialogue orderDialogue)
    {
        string path = "Assets/Resources/Dialogues/" + customerName + "/";
        string filename = "Order";
        string filePath = path + filename + ".asset";

        int orderCount = 1;
        
        // if there is already an existing file named Order.asset
        // name the others like this: Order (n).asset
        while (AssetDatabase.LoadAssetAtPath<Dialogue>(filePath) != null)
        {
            filePath = path + filename + " (" + orderCount + ").asset";
            orderCount++;
        }

        AssetDatabase.CreateAsset(orderDialogue, filePath);
        AssetDatabase.SaveAssets();
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

    // don't directly access the dialogueData.
    // pass dialogue as parameter

    private IEnumerator StartTalking()
    {
        // delay 3 secs
        yield return new WaitForSeconds(2f);

        ChangeCustomerState(CustomerState.Talking);
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
            StopAllCoroutines();
            dialogueController.SetDialogueText(dialogueData.lines[dialogueIndex]);
            isTyping = false;
            return;
        }

        // clear choices
        dialogueController.ClearChoices();

        // check endDialogueLines
        if(dialogueData.endLines.Count > dialogueIndex && dialogueData.endLines[dialogueIndex])
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

        isTyping = false;

        if (dialogueData.linesToAutoProgress.Count > dialogueIndex && dialogueData.linesToAutoProgress[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            // display next line
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
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
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueController.SetDialogueText("");
        dialogueController.ShowDialogueUI(false);
        ChangeCustomerState(CustomerState.Idle);    // waiting
        // stop dialogue from showing again.
        // dialogueData = null;
        if (willBuyAfterSmallTalk)
        {
            willBuyAfterSmallTalk = false;

            // buying time
            dialogueData = orderDialogue;
            StartCoroutine(StartTalking());
        }
        else
        {
            // everything done
            dialogueData = null;

            if (!willBuy)   // only small talk. end already
            {
                isDone = true;
                StartCoroutine(DestroySmallTalkNPC());
            }
        }
    }


    private IEnumerator DestroySmallTalkNPC()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }



    public void ChangeCustomerState(CustomerState newCustomerState)
    {
        customerState = newCustomerState;

        switch (customerState)
        {
            case CustomerState.Idle:
                Debug.Log("Customer is waiting.");
                customerSpriteRenderer.sprite = expressions[1];     // just smile
                break;

            case CustomerState.Talking:
                Debug.Log("Customer is stating his order");
                customerSpriteRenderer.sprite = expressions[0];
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
    HappyExpression,
    NeutralExpression
}
