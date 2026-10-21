using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            
            // if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            // {
            //     return;
            // }
            SoundFXController.Play("Menu");
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
        }
    }

    public void OnExitClick()
    {
        SoundFXController.Play("ThudSound");
        SceneManager.LoadScene("StartScene");
    }
}
