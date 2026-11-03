using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneHandler : MonoBehaviour
{
    public TMP_Text[] preTestTextFields = new TMP_Text[3];
    public TMP_Text[] trainingTextFields = new TMP_Text[3];
    public TMP_Text[] postTestTextFields = new TMP_Text[3];
    public List<int> playerScores;

    private string saveLocation;
    private List<int> preTestScores;
    private List<int> trainingScores;
    private List<int> postTestScores;
    

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (File.Exists(saveLocation))
        {
            playerScores = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation)).completedLevelScores;
        }

        preTestScores = playerScores.GetRange(0,9);
        trainingScores = playerScores.GetRange(9,9);
        postTestScores = playerScores.GetRange(18,9);

        AddScoresToText(preTestScores, preTestTextFields);
        AddScoresToText(trainingScores, trainingTextFields);
        AddScoresToText(postTestScores, postTestTextFields);
    }

    void AddScoresToText(List<int> scores, TMP_Text[] textFields)
    {
        for (int i=0; i<textFields.Length; i++)
        {
            // 3 scores per level
            for (int j=0; j<3; j++)
            {
                textFields[i].text += scores[i * 3 + j];

                if (j < 2)
                {
                    textFields[i].text += ", ";     // don't add comma to the last score
                }
            }
        }
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    Application.Quit();
    }

}
