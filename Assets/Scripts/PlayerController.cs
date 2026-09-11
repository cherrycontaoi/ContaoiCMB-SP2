
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
            eraser.DeactivateEraser();
        }
        else if(rayHit.collider.gameObject.name == "Eraser")
        {

            pencil.DeactivatePencil();
            eraser.ActivateEraser();
        }
        else if(rayHit.collider.gameObject.CompareTag("PlasticBag"))
        {
            counterDisplayController.ChoosePlasticBag(rayHit.collider.gameObject.name);
        }
    }

}
