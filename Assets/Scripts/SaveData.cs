using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Difficulty difficulty;
    public int averageScore, totalScore;
    public int transactionsCompleted, lastFinishedLevel;
    public int preTestAverageScore, postTestAverageScore;
}
