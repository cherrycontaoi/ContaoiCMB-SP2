using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OtherScreensController : MonoBehaviour
{

    public static OtherScreensController Instance { get; private set; }
    public GameObject levelCompleteScreen;
    public GameObject transitionPanel;
    public TMP_Text transitionText;
    public GameObject[] starsContainer;
    private List<Image> starImages = new List<Image>();
    private string[] transitionLines = {
        "Anak, kain na!",
        "Anak, halika't bumili ako ng meryenda.",
        "Palitan na kita, 'nak.",
        "May customer ka? Halika muna dito.",
        "'Nak, ako na dyan. Bukas ka na ulit magbantay."
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    public void ChooseTransitionText()
    {
        int index = Random.Range(0, transitionLines.Length);

        transitionText.text = transitionLines[index];
    }

    public void ShowTransitionDialogue(bool show)
    {
        if (show)
        {
            ChooseTransitionText();
        }
        transitionPanel.SetActive(show);
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnNextClick()
    {
        // just continue
        levelCompleteScreen.SetActive(false);
        LevelController.Instance.StartNextLevel();
    }

    public void ShowScreen()
    {
        levelCompleteScreen.SetActive(true);
    }

    public void HideScreen()
    {
        levelCompleteScreen.SetActive(false);
    }


    public void SetStars(int levelScore)
    {

        starImages.Clear(); 
        
        foreach (GameObject star in starsContainer)
        {
            starImages.Add(star.GetComponent<Image>());
        }

        for (int i=0; i<starImages.Count; i++)
        {
            // float starInNum = levelScore - (i * 100);

            float alpha = (levelScore - (i*100))/100f;

            Color color = starImages[i].color;
            color.a = alpha;
            starImages[i].color = color;
        }

    }


}
