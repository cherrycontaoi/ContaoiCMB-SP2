using UnityEngine;

public class TutorialUIHandler : MonoBehaviour
{
    public GameObject displayNote;
    public GameObject notebookNote;
    public GameObject notepadNote;
    public GameObject plasticbagNote;
    public GameObject customerNote;
    public GameObject bagChoosingNote;

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

        }
    }
}
