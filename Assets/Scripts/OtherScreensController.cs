using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OtherScreensController : MonoBehaviour
{

    public static OtherScreensController Instance { get; private set; }

    // level complete screen
    public GameObject levelCompleteScreen;
    public GameObject[] starsContainer;
    private List<Image> starImages = new List<Image>();

    // end of level transition to level complete screen
    public GameObject transitionPanel;
    public TMP_Text transitionText;
    private string[] transitionLines = {
        "Anak, kain na!",
        "Anak, halika't bumili ako ng meryenda.",
        "Palitan na kita, 'nak.",
        "May customer ka? Halika muna dito.",
        "'Nak, ako na dyan. Bukas ka na ulit magbantay."
    };

    // transition to start of level screen
    // shows narrative with rules and other info
    public GameObject startPanel;

    public GameObject blockageTileset;
    public GameObject storeTakip;
    public GameObject clickReminder;
    public TMP_Text reminderText;

    public Sprite[] paperSprites;
    public GameObject[] paperTileSets;
    public GameObject displayPaper;
    public GameObject paperElements;


    public Transform paperContentWindow;    
    public GameObject paperText;
    private bool hasReadPaper;
    public bool storeIsOpen;
    private int index;

    


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    // FOR TRANSITION TO END ---------------------------------------------------
    public void ChooseTransitionText()
    {
        int index = Random.Range(0, transitionLines.Length);

        transitionText.text = transitionLines[index];
    }

    public void ShowTransitionDialogue(bool show)
    {
        Pencil.Instance.DeactivatePencil();
        Eraser.Instance.DeactivateEraser();
        if (show)
        {
            ChooseTransitionText();
        }
        transitionPanel.SetActive(show);
    }

    // FOR LEVEL COMPLETE! -----------------------------------------------------
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

    // FOR TRANSITION TO START OF LEVEL --------------------------------------

    public void ShowStartAnimation()
    {
        index = Random.Range(0, paperSprites.Length);
        displayPaper.GetComponent<Image>().sprite = paperSprites[index];
        
        hasReadPaper = false;
        storeIsOpen = false;

        displayPaper.SetActive(true);
        blockageTileset.SetActive(true);
        clickReminder.SetActive(true);
        paperElements.SetActive(false);

        startPanel.SetActive(true);
    }

    public void CloseStore()
    {
        storeIsOpen = false;

        blockageTileset.SetActive(true);
        storeTakip.SetActive(true);
        clickReminder.SetActive(true);
        paperElements.SetActive(false);
        PauseController.SetPause(true);
    }

    public void OpenStore()
    {
        if (!hasReadPaper)
        {
            reminderText.text = "Wait! Baka may mga habilin.";
            return;
        }

        storeIsOpen = true;
        blockageTileset.SetActive(false);
        storeTakip.SetActive(false);
        clickReminder.SetActive(false);
        PauseController.SetPause(false);
    }

    public void ReadPaper()
    {
        displayPaper.SetActive(false);
        paperTileSets[index].SetActive(true);
        paperElements.SetActive(true);
    }
    
    public void ClosePaper()
    {
        hasReadPaper = true;
        paperTileSets[index].SetActive(false);
        paperElements.SetActive(false);
        reminderText.text = "I-click yung cover para buksan yung store!";
    }

    public void SetInstructions(string filename)
    {
        string readFromFilePath = "Assets/Resources/Levels/Instructions/" + filename;

        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();
        
        foreach(string line in fileLines)
        {
            GameObject newPaperText = Instantiate(paperText, paperContentWindow);
            newPaperText.GetComponentInChildren<TMP_Text>().text = line;
        }

    }

}
