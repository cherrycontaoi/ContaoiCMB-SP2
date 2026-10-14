using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Difficulty difficulty;
    public List<int> scores = new List<int>();
    public int averageScore, totalScore;
    public int transactionsCompleted, lastFinishedLevel;
    public int preTestAverageScore, postTestAverageScore;
}
