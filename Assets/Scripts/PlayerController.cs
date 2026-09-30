
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private CursorController cursorController;

    [SerializeField] private Pencil pencil;
    [SerializeField] private Eraser eraser;
    [SerializeField] private CounterDisplayController counterDisplayController;
    public GameObject dialoguePanel;

    private void Start()
    {
        counterDisplayController = FindAnyObjectByType<CounterDisplayController>();
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        pencil.DeactivatePencil();
        eraser.DeactivateEraser();
        
    }

    public void Update()
    {
        if(!Mouse.current.leftButton.wasPressedThisFrame) return;

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

        if (!rayHit.collider) return;

        if(rayHit.collider.gameObject.name == "Pencil") {
            // Debug.Log("pencil");
            pencil.ActivatePencil();
            eraser.canActivate = false;
            // eraser.DeactivateEraser();
        }
        else if(rayHit.collider.gameObject.name == "Eraser")
        {
            if (!pencil.isActive)
            {
                eraser.canActivate = true;
            }
            eraser.ActivateEraser();
            // pencil.DeactivatePencil();
        }
        else if(rayHit.collider.gameObject.CompareTag("PlasticBag"))
        {
            if (PauseController.IsGamePaused) return;
            counterDisplayController.ChoosePlasticBag(rayHit.collider.gameObject.name);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        RectTransform rectTransform = dialoguePanel.GetComponent<RectTransform>();
        Customer customer = FindAnyObjectByType<Customer>();

        if (context.performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, null))
            {
                customer.Interact();   
            }
        }
    }

}
