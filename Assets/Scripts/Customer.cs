using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Customer : MonoBehaviour
{

    [SerializeField] Order orderPrefab;
    [SerializeField] private Sprite[] expressions;
    [SerializeField] private string customerName;
    private SpriteRenderer customerSpriteRenderer;
    private CustomerState customerState = CustomerState.Idle;
    [SerializeField] public Order order;

    // Dialogue stuff
    public List<Dialogue> dialogueData;
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
        dialogueData.Add(orderDialogue);


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

            case CustomerState.StatingOrder:
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
}

public enum CustomerState
{
    Idle,
    StatingOrder,
    HappyExpression,
    NeutralExpression
}
