using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;

    private void Reset()
    {
        textField = GetComponentInChildren<TMP_Text>();
    }

    public void SetNote(string text)
    {
        textField.SetText(text);
    }
    
}
