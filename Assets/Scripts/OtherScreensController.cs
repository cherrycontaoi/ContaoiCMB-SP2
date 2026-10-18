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
    public GameObject nextLevelButton;
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
    public GameObject clearNotepadButton;
    public GameObject bilihinContainer;


    public Transform paperContentWindow;    
    public GameObject paperText;
    public GameObject paperImage;
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
        SoundFXController.Play("TransitionToEnd");
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
        SoundFXController.Play("ThudSound");
        SceneManager.LoadScene("StartScene");
    }

    public void OnNextClick()
    {
        // just continue
        SoundFXController.Play("ThudSound");
        levelCompleteScreen.SetActive(false);
        LevelController.Instance.StartNextLevel();
    }

    public void ShowScreen()
    {
        SoundFXController.Play("LevelComplete");
        levelCompleteScreen.SetActive(true);

        int lastLevelIndex = LevelController.Instance.levels.Length-1;
        if (LevelController.Instance.currentLevel == LevelController.Instance.levels[lastLevelIndex])
        {
            nextLevelButton.SetActive(false);
        }
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
        if (Eraser.Instance.isActive) return;

        if (!hasReadPaper)
        {
            reminderText.text = "Wait! Baka may mga bilin.";
            return;
        }

        SoundFXController.Play("OpenStore");
        storeIsOpen = true;
        blockageTileset.SetActive(false);
        storeTakip.SetActive(false);
        clickReminder.SetActive(false);
        PauseController.SetPause(false);
    }

    public void ReadPaper()
    {
        if (Eraser.Instance.isActive) return;
        
        SoundFXController.Play("ReadPaper");
        displayPaper.SetActive(false);

        bilihinContainer.SetActive(false);
        clearNotepadButton.SetActive(false);

        paperTileSets[index].SetActive(true);
        paperElements.SetActive(true);
        
    }
    
    public void ClosePaper()
    {
        SoundFXController.Play("Paper");
        hasReadPaper = true;

        paperTileSets[index].SetActive(false);
        paperElements.SetActive(false);

        if (LevelController.Instance.currentLevel.levelType != LevelType.PreTest)
        {
            bilihinContainer.SetActive(true);
            clearNotepadButton.SetActive(true);
        }

        reminderText.text = "I-click ang cover para buksan ang store!";
    }

    public void SetInstructions(string filename)
    {

        foreach (Transform child in paperContentWindow)
        {
            Destroy(child.gameObject);    
        }

        TextAsset file = Resources.Load<TextAsset>("Instructions/" + filename);

        if (file == null)
        {
            Debug.LogError("Couldn't load instruction file with name: " +filename);
            return;
        }

        List<string> fileLines = file.text
        .Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.None)
        .ToList();

        foreach (string line in fileLines)
        {
            if (line.StartsWith("img_src="))
            {
                string imgPath = line.Replace("img_src=", "").Trim();

                Sprite instructionPhoto = Resources.Load<Sprite>(imgPath);

                if (instructionPhoto == null)
                {
                    Debug.LogError("Could not load instruction image: " + imgPath);
                    continue;
                }

                GameObject newPaperPhoto = Instantiate(paperImage, paperContentWindow);

                newPaperPhoto.GetComponentInChildren<Image>().sprite = instructionPhoto;
            }
            else
            {
                GameObject newPaperText = Instantiate(paperText, paperContentWindow);
                newPaperText.GetComponentInChildren<TMP_Text>().text = line;
            }
        }
    }

    //     string readFromFilePath = "Assets/Resources/Instructions/" + filename;

    //     List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();
        
    //     foreach(string line in fileLines)
    //     {
    //         if (line.StartsWith("img_src="))
    //         {
    //             string imgPath = line.Replace("img_src=", "");

    //             Sprite instructionPhoto = Resources.Load<Sprite>(imgPath);
    //             GameObject newPaperPhoto = Instantiate(paperImage, paperContentWindow);

    //             newPaperPhoto.GetComponentInChildren<Image>().sprite = instructionPhoto;
    //         }
    //         else
    //         {
    //             GameObject newPaperText = Instantiate(paperText, paperContentWindow);
    //             newPaperText.GetComponentInChildren<TMP_Text>().text = line;
    //         }

    //     }

    // }

}
