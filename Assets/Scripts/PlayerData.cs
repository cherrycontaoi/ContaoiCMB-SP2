using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private Difficulty playerPuzzleLevels;     // varies
    [SerializeField] private int averageScore;                   // = totalScore/transactionsCompleted, determines puzzle level
    [SerializeField] private int totalScore;
    [SerializeField] private int transactionsCompleted;          // ilang customers nacomplete
    private int preTestAverageScore, postTestAverageScore;

    
    void Start()
    {
        playerPuzzleLevels = Difficulty.Easy;
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

    public void AddFinishedTransaction(int score)
    {
        totalScore += score;
        transactionsCompleted++;

        UpdateAverageScore();
        UpdatePlayerLevel();
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