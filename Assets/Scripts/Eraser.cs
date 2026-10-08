
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Eraser : MonoBehaviour
{
    // TO DO: NEED TO INCREASE SIZE OF ERASER CURSOR!!!!

    public static Eraser Instance { get; private set; }
    private Camera mainCamera;
    public bool isActive;
    [SerializeField] private CursorController cursorController;
    [SerializeField] public GameObject eraserReminder;
    public bool canActivate = true;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!isActive) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
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
        if (canActivate)
        {
            isActive = true;
            // PauseController.SetPause(true);
            cursorController.SetToEraser();
            eraserReminder.SetActive(true);
        }

    }

    public void DeactivateEraser()
    {
        isActive = false;
        // PauseController.SetPause(false);
        cursorController.SetToDefault();
        eraserReminder.SetActive(false);
    }
}
