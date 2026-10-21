using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Difficulty difficulty;
    public List<int> completedLevelScores = new List<int>();
    public int averageScore, totalScore;
    public int transactionsCompleted, lastFinishedLevel;
    public int preTestTotalScore, postTestTotalScore;
    public int preTestAverageScore, postTestAverageScore;
    public int preTestCompleted, postTestCompleted;

}
