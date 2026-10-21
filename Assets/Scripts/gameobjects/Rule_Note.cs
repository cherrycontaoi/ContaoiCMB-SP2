using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rule_Note : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text textField;

    private void Reset()
    {
        textField = GetComponent<TMP_Text>();
    }

    public void SetNote(string text)
    {
        textField.SetText(text);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Eraser.Instance.isActive) return;
        SoundFXController.Play("Eraser");
        Destroy(gameObject);
        Debug.Log("A bilin note was deleted!");
    }
    
}
