using UnityEngine;

public class Pencil : MonoBehaviour
{
    [SerializeField] public GameObject createNotePanel;
    [SerializeField] public GameObject pencilReminder;


    public bool isActive;

    public void ActivatePencil()
    {
        isActive = true;
        // PauseController.SetPause(true);
        createNotePanel.SetActive(true);
        pencilReminder.SetActive(true);
    }

    public void DeactivatePencil()
    {
        isActive = false;
        // PauseController.SetPause(false);
        createNotePanel.SetActive(false);
        pencilReminder.SetActive(false);
    }
}
