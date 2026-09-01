using TMPro;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;

    private BoxCollider2D boxCollider;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Reset()
    {
        textField = GetComponent<TMP_Text>();
    }

    private void UpdateColliderSize()
    {
        boxCollider.size = new Vector2(
            rectTransform.rect.width,
            rectTransform.rect.height/4f
        );
        boxCollider.offset = new Vector2(0f, -80f); // estimate
    }
        private void LateUpdate()
    {
        UpdateColliderSize();
    }

    public void SetNote(string text)
    {
        textField.SetText(text);
    }
    
}
