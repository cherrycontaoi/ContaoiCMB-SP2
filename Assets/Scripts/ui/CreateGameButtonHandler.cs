using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateGameButtonHandler : MonoBehaviour
{
    public void OnNewGameClick()
    {
        SoundFXController.Play("ThudSound");
        PlayerData.Instance.ResetPlayerData();
        SaveController.Instance.SaveGame();
        SceneManager.LoadScene("StartCutscene");
        
    }
}
