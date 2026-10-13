using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public Difficulty playerPuzzleLevels;     // varies
    public int averageScore;                   // = totalScore/transactionsCompleted, determines puzzle level
    public int totalScore;
    public int transactionsCompleted;          // ilang customers nacomplete
    
    public int lastFinishedLevel = 0;
    public int preTestTotalScore, postTestTotalScore = 0;
    public int preTestAverageScore, postTestAverageScore = 0;
    public int preTestCompleted, postTestCompleted = 0;

    public TMP_Text aveScoreText;
    public TMP_Text transactionsText;
    public TMP_Text difficultyText;
    public TMP_Text averagePreTestText;
    public TMP_Text averagePostTestText;

    
    void Start()
    {
        UpdatePlayerInfo();
    }

    private void UpdatePlayerLevel()
    {
        if (averageScore <= 50)
        {
            playerPuzzleLevels = Difficulty.Easy;
        }
        else if (averageScore > 50 && averageScore <= 75)
        {
            playerPuzzleLevels = Difficulty.Medium;
        }
        else
        {
            playerPuzzleLevels = Difficulty.Hard;
        }

    }

    private void UpdateAverageScore(ref int aveCounter, int typeTotalScore, int divider)
    {
        aveCounter = typeTotalScore / divider;
    }

    private void UpdatePlayerInfo()
    {
        difficultyText.SetText(playerPuzzleLevels.ToString());
        aveScoreText.SetText(averageScore.ToString());
        transactionsText.SetText(transactionsCompleted.ToString());
    }

    public void AddFinishedTransaction(int score)
    {
        totalScore += score;
        transactionsCompleted++;

        UpdateAverageScore(ref averageScore, totalScore, transactionsCompleted);
        UpdatePlayerLevel();
        UpdatePlayerInfo();
    }

    public void AddPreTestScore(int score)
    {
        preTestTotalScore += score;
        totalScore += score;

        preTestCompleted++;
        transactionsCompleted++;

        UpdateAverageScore(ref preTestAverageScore, preTestTotalScore, preTestCompleted);
        averagePreTestText.SetText(preTestAverageScore.ToString());

        UpdateAverageScore(ref averageScore, totalScore, transactionsCompleted);
        UpdatePlayerLevel();
        UpdatePlayerInfo();
    }

    public void AddPostTestScore(int score)
    {
        postTestTotalScore += score;
        totalScore += score;
        
        postTestCompleted++;
        transactionsCompleted++;
        
        UpdateAverageScore(ref postTestAverageScore, postTestTotalScore, postTestCompleted);
        averagePostTestText.SetText(postTestAverageScore.ToString());

        UpdateAverageScore(ref averageScore, totalScore, transactionsCompleted);
        UpdatePlayerLevel();
        UpdatePlayerInfo();
    }

    public Difficulty GetPlayerPuzzleLevel()
    {
        return playerPuzzleLevels;
    }

}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    TestMode
}
