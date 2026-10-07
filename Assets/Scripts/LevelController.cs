using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }
    public Level[] levels;
    public int[] levelsCompleted;      // Level.levelNo

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
        levels[currentLevelIndex].StartLevel();
 
    }

    public void StartNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            OtherScreensController.Instance.HideScreen();
            levels[currentLevelIndex].StartLevel();
        }
        else
        {
            Debug.Log("No more levels left!");
        }
    }
}
