using UnityEngine;

public class Pencil : MonoBehaviour
{
    [SerializeField] public GameObject createNotePanel;
    [SerializeField] public GameObject pencilReminder;


    private bool isActive;

    public void Write()
    {
        // enable add note/order
        createNotePanel.SetActive(true);
        pencilReminder.SetActive(true);

    }

    public void ActivatePencil()
    {
        isActive = true;
        Write();
    }

    public void DeactivatePencil()
    {
        isActive = false;
        createNotePanel.SetActive(false);
        pencilReminder.SetActive(false);
    }
}
