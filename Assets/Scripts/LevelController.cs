using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }
    public Level[] levels;
    public int[] levelsCompleted;      // Level.levelNo
    public Level currentLevel;
    public GameObject notetakingSpace;
    private int currentLevelIndex;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCurrentLevel();
    }

    private void StartCurrentLevel()
    {
        OtherScreensController.Instance.HideScreen();
        currentLevel = levels[currentLevelIndex];

        if (currentLevel.levelType == LevelType.PreTest)
        {
            Pencil.Instance.gameObject.SetActive(false);
            Eraser.Instance.gameObject.SetActive(false);
            notetakingSpace.SetActive(false);
        }
        else
        {
            Pencil.Instance.gameObject.SetActive(true);
            Eraser.Instance.gameObject.SetActive(true);
            notetakingSpace.SetActive(true);
        }

        levels[currentLevelIndex].StartLevel();
    }

    public void StartNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            OtherScreensController.Instance.HideScreen();
            currentLevel = levels[currentLevelIndex];
            levels[currentLevelIndex].StartLevel();
        }
        else
        {
            Debug.Log("No more levels left!");
        }
    }
}
