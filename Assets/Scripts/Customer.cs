using TMPro;
using UnityEngine;

public class Customer : MonoBehaviour
{

    [SerializeField] Order orderPrefab;
    [SerializeField] private Sprite[] expressions;
    private SpriteRenderer customerSpriteRenderer;
    private CustomerState customerState = CustomerState.Idle;
    [SerializeField] public Order order;

    // Dialogue stuff
    public Dialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    void Start()
    {
        transform.position = new Vector3(-5f, 3.61f, 1f);   // position ng orderan
        order = Instantiate(orderPrefab, transform);
        customerSpriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(gameObject.name + "'s current state is: " + customerState);
        ChangeCustomerState(CustomerState.Idle);
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
}

public enum CustomerState
{
    Idle,
    StatingOrder,
    HappyExpression,
    NeutralExpression
}
