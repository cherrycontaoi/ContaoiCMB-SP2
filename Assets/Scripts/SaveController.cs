using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private PlayerData playerData;
    private string saveLocation;
    
    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();
    }

    public void SaveGame()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        SaveData saveData = new SaveData
        {
            difficulty = playerData.playerPuzzleLevels,
            averageScore = playerData.averageScore,
            totalScore = playerData.totalScore,
            transactionsCompleted = playerData.transactionsCompleted,
            lastFinishedLevel = playerData.lastFinishedLevel,
            preTestAverageScore = playerData.preTestAverageScore,
            postTestAverageScore = playerData.postTestAverageScore
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            playerData = FindAnyObjectByType<PlayerData>();
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            playerData.playerPuzzleLevels = saveData.difficulty;
            playerData.averageScore = saveData.averageScore;
            playerData.totalScore = saveData.totalScore;
            playerData.transactionsCompleted = saveData.transactionsCompleted;
            playerData.lastFinishedLevel = saveData.lastFinishedLevel;
            playerData.preTestAverageScore = saveData.postTestAverageScore;
            playerData.postTestAverageScore = saveData.postTestAverageScore;

        }
        else
        {
            SaveGame();
        }
    }
}
