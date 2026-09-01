using UnityEngine;

public class Pencil : MonoBehaviour
{
    [SerializeField]
    public GameObject createNotePanel;

    private bool isActive;

    public void Write()
    {
        // enable add note/order
        createNotePanel.SetActive(true);
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
    }
}
