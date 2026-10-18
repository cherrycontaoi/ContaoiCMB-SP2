using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance;
    private string saveLocation;
    
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            LoadGame();
        }
    }

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else Destroy(gameObject);   // make only 1 instance
    }
    

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            difficulty = PlayerData.Instance.playerPuzzleLevels,
            lastFinishedLevel = PlayerData.Instance.lastFinishedLevel,
            completedLevelScores = PlayerData.Instance.completedLevelScores,

            averageScore = PlayerData.Instance.averageScoreInCompletedAll,
            totalScore = PlayerData.Instance.totalScoreInCompletedLevels,
            transactionsCompleted = PlayerData.Instance.totalTransactionsInCompletedLevels,

            preTestAverageScore = PlayerData.Instance.averageScoreInCompletedPre,
            preTestTotalScore = PlayerData.Instance.totalScoreInCompletedPreLevels,
            preTestCompleted = PlayerData.Instance.totalCompletedPreTransactions,

            postTestAverageScore = PlayerData.Instance.averageScoreInCompletedPost,
            postTestTotalScore = PlayerData.Instance.totalScoreInCompletedPostLevels,            
            postTestCompleted = PlayerData.Instance.totalCompletedPostTransactions,

        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            PlayerData.Instance.playerPuzzleLevels = saveData.difficulty;
            PlayerData.Instance.lastFinishedLevel = saveData.lastFinishedLevel;
            PlayerData.Instance.completedLevelScores = saveData.completedLevelScores;

            PlayerData.Instance.averageScoreInCompletedAll = saveData.averageScore;
            PlayerData.Instance.totalScoreInCompletedLevels = saveData.totalScore;
            PlayerData.Instance.totalTransactionsInCompletedLevels = saveData.transactionsCompleted;
            
            PlayerData.Instance.averageScoreInCompletedPre = saveData.preTestAverageScore;
            PlayerData.Instance.totalScoreInCompletedPreLevels = saveData.preTestTotalScore;
            PlayerData.Instance.totalCompletedPreTransactions = saveData.preTestCompleted;

            PlayerData.Instance.averageScoreInCompletedPost = saveData.postTestAverageScore;            
            PlayerData.Instance.totalScoreInCompletedPostLevels = saveData.postTestTotalScore;            
            PlayerData.Instance.totalCompletedPostTransactions = saveData.postTestCompleted;



            PlayerData.Instance.averageScore = PlayerData.Instance.averageScoreInCompletedAll;
            PlayerData.Instance.totalScore = PlayerData.Instance.totalScoreInCompletedLevels;
            PlayerData.Instance.transactionsCompleted = PlayerData.Instance.totalTransactionsInCompletedLevels;

            PlayerData.Instance.preTestAverageScore = PlayerData.Instance.averageScoreInCompletedPre;
            PlayerData.Instance.preTestTotalScore = PlayerData.Instance.totalScoreInCompletedPreLevels;
            PlayerData.Instance.preTestCompleted = PlayerData.Instance.totalCompletedPreTransactions;

            PlayerData.Instance.postTestAverageScore = PlayerData.Instance.averageScoreInCompletedPost;
            PlayerData.Instance.postTestTotalScore = PlayerData.Instance.totalScoreInCompletedPostLevels;
            PlayerData.Instance.postTestCompleted = PlayerData.Instance.totalCompletedPostTransactions;

            PlayerData.Instance.UpdatePlayerInfo();
        }
        else
        {
            SaveGame();
        }
    }
}
