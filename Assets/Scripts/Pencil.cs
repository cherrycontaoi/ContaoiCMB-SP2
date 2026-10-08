using UnityEngine;

public class Pencil : MonoBehaviour
{

    public static Pencil Instance { get; private set; }
    [SerializeField] public GameObject createNotePanel;
    [SerializeField] public GameObject pencilReminder;


    public bool isActive;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

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
