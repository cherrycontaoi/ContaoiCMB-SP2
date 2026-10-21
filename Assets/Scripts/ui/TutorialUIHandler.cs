using UnityEngine;

public class TutorialUIHandler : MonoBehaviour
{
    public GameObject displayNote;
    public GameObject notebookNote;
    public GameObject notepadNote;
    public GameObject plasticbagNote;
    public GameObject customerNote;
    public GameObject bagChoosingNote;
    public GameObject moneyGuide;
    public GameObject money;
    public GameObject[] plasticBagGrids;

    private void Update()
    {
        if (OtherScreensController.Instance.storeIsOpen)
        {
            displayNote.SetActive(true);
            notebookNote.SetActive(true);
            notepadNote.SetActive(true);
            plasticbagNote.SetActive(true);
            customerNote.SetActive(true);
            bagChoosingNote.SetActive(true);

            foreach(GameObject grid in plasticBagGrids)
            {
                if (grid.activeSelf)
                {
                    bagChoosingNote.SetActive(false);
                    break;
                }
            }

            if (money.activeSelf)
            {
                moneyGuide.SetActive(true);
            }
            else
            {
                moneyGuide.SetActive(false);
            }
        }
    }
}
