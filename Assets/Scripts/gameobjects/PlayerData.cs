using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    public Difficulty playerPuzzleLevels;     // varies

    // In-Game stuff --------------------------
    public int averageScore;                   // = totalScore/transactionsCompleted, determines puzzle level
    public int totalScore;
    public int transactionsCompleted;          // ilang customers nacomplete
    
    public int lastFinishedLevel = -1;  // becomes 0 if first level is completed
    public int preTestTotalScore, postTestTotalScore ;          // total
    public int preTestAverageScore, postTestAverageScore;       // ave
    public int preTestCompleted, postTestCompleted;             // count


    // USED IN SAVING THE LAST COMPLETED PROGRESS ------------------------

    // overall
    public int averageScoreInCompletedAll;
    public int totalScoreInCompletedLevels;     // only the scores in the completed levels will be added here
    public int totalTransactionsInCompletedLevels;  // completed level transactions only
     
    // pre
    public int averageScoreInCompletedPre;
    public int totalScoreInCompletedPreLevels;
    public int totalCompletedPreTransactions;

    // post
    public int averageScoreInCompletedPost;
    public int totalScoreInCompletedPostLevels;
    public int totalCompletedPostTransactions;

    public List<int> completedLevelScores = new List<int>();


    // UI -----------------------------------------------------

    public TMP_Text aveScoreText;
    public TMP_Text transactionsText;
    public TMP_Text difficultyText;
    public TMP_Text averagePreTestText;
    public TMP_Text averagePostTestText;

    
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "TutorialScene") UpdatePlayerInfo();
    }
    
    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void UpdatePlayerLevel()
    {
        if (LevelController.Instance.currentLevel.levelType == LevelType.PreTest || 
            LevelController.Instance.currentLevel.levelType == LevelType.PostTest)
        {
            playerPuzzleLevels = Difficulty.Test;
        }

        else
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
    }

    private void UpdateAverageScore(ref int aveCounter, int typeTotalScore, int divider)
    {
        aveCounter = typeTotalScore / divider;
    }

    public void UpdatePlayerInfo()
    {
        difficultyText.SetText(playerPuzzleLevels.ToString());
        aveScoreText.SetText(averageScore.ToString());
        averagePreTestText.SetText(preTestAverageScore.ToString());
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

    // data here will be the one to be saved!!
    public void AddCompletedLevel(int score, List<int> levelScores, LevelType levelType)
    {
        if (levelType == LevelType.PreTest)
        {
            totalScoreInCompletedPreLevels += score;
            totalCompletedPreTransactions += 3;     // 3 customer per level

            averageScoreInCompletedPre = totalScoreInCompletedPreLevels / totalCompletedPreTransactions;
        }

        else if (levelType == LevelType.PostTest)
        {
            totalScoreInCompletedPostLevels += score;
            totalCompletedPostTransactions += 3;

            averageScoreInCompletedPost = totalScoreInCompletedPostLevels / totalCompletedPostTransactions;
        }

        totalScoreInCompletedLevels += score;
        totalTransactionsInCompletedLevels += 3;

        averageScoreInCompletedAll = totalScoreInCompletedLevels / totalTransactionsInCompletedLevels;

        completedLevelScores.AddRange(levelScores);
        UpdatePlayerLevel();
    }

    public void ResetPlayerData()
    {
        playerPuzzleLevels = Difficulty.Test;
        lastFinishedLevel = -1;
        completedLevelScores = new List<int>();

        averageScoreInCompletedAll = 0;
        totalScoreInCompletedLevels = 0;
        totalTransactionsInCompletedLevels = 0;

        averageScoreInCompletedPre = 0;
        totalScoreInCompletedPreLevels = 0;
        totalCompletedPreTransactions = 0;

        averageScoreInCompletedPost = 0;
        totalScoreInCompletedPostLevels = 0;
        totalCompletedPostTransactions = 0;

        averageScore = 0;
        totalScore = 0;
        transactionsCompleted = 0;

        preTestAverageScore = 0;
        preTestTotalScore = 0;
        preTestCompleted = 0;

        postTestAverageScore = 0;
        postTestTotalScore = 0;
        postTestCompleted = 0;

        UpdatePlayerInfo();
    }

}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Test
}
