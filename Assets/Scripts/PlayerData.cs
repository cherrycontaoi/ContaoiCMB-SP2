using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private PuzzleLevel playerPuzzleLevels;     // varies
    [SerializeField] private int averageScore;                   // = totalScore/transactionsCompleted, determines puzzle level
    [SerializeField] private int totalScore;
    [SerializeField] private int transactionsCompleted;          // ilang customers nacomplete
    private int preTestAverageScore, postTestAverageScore;

    
    void Start()
    {
        playerPuzzleLevels = PuzzleLevel.Easy;
    }

    private void UpdatePlayerLevel()
    {
        if (averageScore <= 50)
        {
            playerPuzzleLevels = PuzzleLevel.Easy;
        }
        else if (averageScore > 50 && averageScore <= 75)
        {
            playerPuzzleLevels = PuzzleLevel.Medium;
        }
        else
        {
            playerPuzzleLevels = PuzzleLevel.Hard;
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

    public PuzzleLevel GetPlayerPuzzleLevel()
    {
        return playerPuzzleLevels;
    }

}

public enum PuzzleLevel
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