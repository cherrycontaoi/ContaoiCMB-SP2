using TMPro;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] public Difficulty playerPuzzleLevels;     // varies
    [SerializeField] public int averageScore;                   // = totalScore/transactionsCompleted, determines puzzle level
    [SerializeField] public int totalScore;
    [SerializeField] public int transactionsCompleted;          // ilang customers nacomplete
    
    public int lastFinishedLevel = 0;
    public int preTestAverageScore, postTestAverageScore = 0;

    public TMP_Text aveScoreText;
    public TMP_Text transactionsText;
    public TMP_Text difficultyText;

    
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

    private void UpdateAverageScore()
    {
        averageScore = totalScore / transactionsCompleted;
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

        UpdateAverageScore();
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
    Hard
}

public enum PlayerGameMode
{
    PreTest,
    Training,
    PostTest
}