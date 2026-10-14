using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            difficulty = PlayerData.Instance.playerPuzzleLevels,
            scores = PlayerData.Instance.scores,
            averageScore = PlayerData.Instance.averageScore,
            totalScore = PlayerData.Instance.totalScore,
            transactionsCompleted = PlayerData.Instance.transactionsCompleted,
            lastFinishedLevel = PlayerData.Instance.lastFinishedLevel,
            preTestAverageScore = PlayerData.Instance.preTestAverageScore,
            postTestAverageScore = PlayerData.Instance.postTestAverageScore
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            PlayerData.Instance.playerPuzzleLevels = saveData.difficulty;
            PlayerData.Instance.scores = saveData.scores;
            PlayerData.Instance.averageScore = saveData.averageScore;
            PlayerData.Instance.totalScore = saveData.totalScore;
            PlayerData.Instance.transactionsCompleted = saveData.transactionsCompleted;
            PlayerData.Instance.lastFinishedLevel = saveData.lastFinishedLevel;
            PlayerData.Instance.preTestAverageScore = saveData.postTestAverageScore;
            PlayerData.Instance.postTestAverageScore = saveData.postTestAverageScore;

        }
        else
        {
            SaveGame();
        }
    }
}
