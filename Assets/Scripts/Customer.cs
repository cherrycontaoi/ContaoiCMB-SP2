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

    // Dialogue stuff
    // public List<Dialogue> dialogueData;
    public Dialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    
    private int dialogueIndex;
    private bool isTyping, isDialogueActive, hasSmallTalk;

    void Start()
    {
        transform.position = new Vector3(-5f, 3.61f, 1f);   // position ng orderan

        order = Instantiate(orderPrefab, transform);
        order.GenerateOrder();

        customerSpriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(gameObject.name + "'s current state is: " + customerState);
        ChangeCustomerState(CustomerState.Idle);

        Dialogue orderDialogue = DialogueBuilder.BuildOrderDialogue(order, false);

        // add created order dialogue to assets
        SaveNewOrderDialogue(orderDialogue);

        // add order dialogue to dialogues;
        // dialogueData.Add(orderDialogue);
        dialogueData = orderDialogue;

        StartCoroutine(StartTalking());
        

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
            // start dialogue
            StartDialogue();
        }
    }

    private IEnumerator StartTalking()
    {
        // delay 3 secs
        yield return new WaitForSeconds(2f);

        ChangeCustomerState(CustomerState.Talking);
        // already active with the first line in the dialogue;
        isDialogueActive = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);
        // PauseController.SetPause(false);     //true - prevents things from happening

        //type line
        StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (isTyping) 
        {
            // skip the typing animation and show the full line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.lines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.lines.Count)  // still have a line
        {
            StartCoroutine(TypeLine()); // type next line
        }
        else
        {
            // end the dialogue - no more lines
            EndDialogue();
            ChangeCustomerState(CustomerState.Idle);    // waiting
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach(char letter in dialogueData.lines[dialogueIndex])
        {
            dialogueText.text += letter;
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

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        
    }

}

public enum CustomerState
{
    Idle,
    Talking,
    HappyExpression,
    NeutralExpression
}
