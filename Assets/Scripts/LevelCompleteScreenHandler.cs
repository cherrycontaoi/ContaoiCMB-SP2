using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteScreenHandler : MonoBehaviour
{

    public static LevelCompleteScreenHandler Instance { get; private set; }
    public GameObject levelCompleteScreen;
    public GameObject[] starsContainer;
    private List<Image> starImages = new List<Image>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OnNextClick()
    {
        // just continue
        levelCompleteScreen.SetActive(false);
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
