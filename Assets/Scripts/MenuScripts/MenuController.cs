using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{

    public GameObject menuCanvas;
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        // escape to open/close menu
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}
