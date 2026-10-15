using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        SoundFXController.Play("ThudSound");
        SceneManager.LoadScene("StoreScene");
    }

    public void OnTutorialClick()
    {
        SoundFXController.Play("ThudSound");
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnExitClick()
    {
        SoundFXController.Play("ThudSound");
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    Application.Quit();
    }
}
