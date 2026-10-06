using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }
    public Level[] levels;
    public int[] levelsCompleted;      // Level.levelNo

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        foreach(Level level in levels)
        {
            LevelCompleteScreenHandler.Instance.HideScreen();
            level.StartLevel();
            // add transition
        }
    }
}
