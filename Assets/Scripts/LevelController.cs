using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }
    public Level[] levels;
    public int[] levelsCompleted;      // Level.levelNo
    public Level currentLevel;
    public GameObject notetakingSpace;
    public GameObject resetBilinButton;
    public GameObject resetBilihinButton;
    public GameObject createNewGamePanel;
    public int currentLevelIndex;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentLevelIndex = PlayerData.Instance.lastFinishedLevel+1;    // move to the next level

        if (currentLevelIndex < levels.Length)
        {
            StartCurrentLevel();
        }
        else if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            currentLevelIndex = 0;
            StartCurrentLevel();
        }
        else
        {
            createNewGamePanel.SetActive(true);
            Debug.Log("No more levels left!");
        }
    }

    private void StartCurrentLevel()
    {
        OtherScreensController.Instance.HideScreen();
        currentLevel = levels[currentLevelIndex];

        AllowAnnotation(currentLevel.levelType);

        levels[currentLevelIndex].StartLevel();
    }

    public void StartNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            OtherScreensController.Instance.HideScreen();
            currentLevel = levels[currentLevelIndex];
            AllowAnnotation(currentLevel.levelType);
            levels[currentLevelIndex].StartLevel();
        }
        else
        {
            SaveController.Instance.SaveGame();
            SceneManager.LoadScene("EndCutscene");
            Debug.Log("No more levels left!");
        }
    }

    private void AllowAnnotation(LevelType levelType)
    {
        if (levelType == LevelType.PreTest)
        {
            PlayerData.Instance.playerPuzzleLevels = Difficulty.Test;
            // PlayerData.Instance.UpdatePlayerInfo();
            Pencil.Instance.gameObject.SetActive(false);
            Eraser.Instance.gameObject.SetActive(false);
            notetakingSpace.SetActive(false);
            resetBilihinButton.SetActive(false);
            resetBilinButton.SetActive(false);
        }
        else
        {
            Pencil.Instance.gameObject.SetActive(true);
            Eraser.Instance.gameObject.SetActive(true);
            notetakingSpace.SetActive(true);
            resetBilihinButton.SetActive(true);
            resetBilinButton.SetActive(true);
        }
    }
}
