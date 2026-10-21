
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Eraser : MonoBehaviour
{
    public static Eraser Instance { get; private set; }
    private Camera mainCamera;
    public bool isActive;
    [SerializeField] private CursorController cursorController;
    public GameObject eraserReminder;
    public GameObject eraserDisabled;
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
            // Erase();
        }
        
    }
    // public void Erase()
    // {
    //     var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
    
    //     if (!rayHit.collider) {
    //         Debug.Log("nothing here."); 
    //         return;
    //         }

    //     if(rayHit.collider.gameObject.CompareTag("Note"))
    //     {
    //         Debug.Log("A note was deleted.");
    //         Destroy(rayHit.collider.gameObject); //erase
    //     }

    // }
    public void ActivateEraser()
    {
        if (canActivate)
        {   
            if (TransactionController.Instance.customer != null && TransactionController.Instance.customer.IsCustomerTalking())
            {
                Instance.ShowDisabledReminder();
                return;
            }

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

    public void ShowDisabledReminder()
    {
        eraserDisabled.SetActive(true);
        StartCoroutine(HideDisabledReminder());
    }

    public IEnumerator HideDisabledReminder()
    {
        yield return new WaitForSeconds(3f);
        eraserDisabled.SetActive(false);
    }
}
