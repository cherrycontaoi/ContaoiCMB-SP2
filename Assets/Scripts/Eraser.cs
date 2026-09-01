
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Eraser : MonoBehaviour
{
    // TO DO: NEED TO INCREASE SIZE OF ERASER CURSOR!!!!

    private Camera mainCamera;
    private bool isActive;
    [SerializeField] private CursorController cursorController;


    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!isActive) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DeactivateEraser();
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Clicked!");
            Erase();
        }
        
    }
    public void Erase()
    {
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
    
        if (!rayHit.collider) {
            Debug.Log("nothing here."); 
            return;
            }

        if(rayHit.collider.gameObject.CompareTag("Note"))
        {
            Debug.Log("A note was deleted.");
            Destroy(rayHit.collider.gameObject); //erase
        }

    }
    public void ActivateEraser()
    {
        isActive = true;
        cursorController.SetToEraser();
    }

    public void DeactivateEraser()
    {
        isActive = false;
        cursorController.SetToDefault();
    }
}
