using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    private string saveLocation;
    public GameObject startOptionsPanel;

    private void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void OnStartClick()
    {
        SoundFXController.Play("ThudSound");
        if (File.Exists(saveLocation))
        {
            startOptionsPanel.SetActive(true);
        }
        else
        {   // new
            startOptionsPanel.SetActive(false);
            SceneManager.LoadScene("StartCutscene");
        }
    }

    public void OnNewGameClick()
    {
        SoundFXController.Play("ThudSound");
        File.Delete(saveLocation);
        SceneManager.LoadScene("StartCutscene");
        
    }

    public void OnContinueGameClick()
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
