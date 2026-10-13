
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private CursorController cursorController;

    
    [SerializeField] private CounterDisplayController counterDisplayController;
    public GameObject dialoguePanel;
    public GameObject notesPanel;

    private void Start()
    {
        counterDisplayController = FindAnyObjectByType<CounterDisplayController>();
        Pencil.Instance.DeactivatePencil();
        Eraser.Instance.DeactivateEraser();
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Update()
    {
        if(!Mouse.current.leftButton.wasPressedThisFrame) return;

        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));

        if (!rayHit.collider) return;

        if(rayHit.collider.gameObject.name == "Pencil" && LevelController.Instance.currentLevel.levelType != LevelType.PreTest) {
            // Debug.Log("pencil");
            Pencil.Instance.ActivatePencil();
            Eraser.Instance.canActivate = false;
            // eraser.DeactivateEraser();
        }
        else if(rayHit.collider.gameObject.name == "Eraser" && LevelController.Instance.currentLevel.levelType != LevelType.PreTest)
        {
            if (!Pencil.Instance.isActive)
            {
                Eraser.Instance.canActivate = true;
            }
            Eraser.Instance.ActivateEraser();
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
        RectTransform dialogueRectTransform = dialoguePanel.GetComponent<RectTransform>();
        RectTransform noteRectTransform = notesPanel.GetComponent<RectTransform>();

        Customer customer = FindAnyObjectByType<Customer>();

        if (context.performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // if clicked on the dialogue box
            bool isDialoguePanelActive = RectTransformUtility.RectangleContainsScreenPoint(dialogueRectTransform, mousePosition, null);

            // if clicked on the create note panel (which can overlap with dialogue box)
            bool isNotePanelActive = notesPanel.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(noteRectTransform, mousePosition, null);

            // prevents dialogue from moving on if note panel is on top of it and player clicked
            if (isDialoguePanelActive && !isNotePanelActive && customer != null && dialoguePanel.activeInHierarchy)
            {
                customer.Interact();   
            }
            
        }
    }

}
