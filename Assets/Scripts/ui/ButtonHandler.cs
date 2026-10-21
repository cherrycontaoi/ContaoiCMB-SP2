using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public TMP_Text buttonText;
    private Image buttonImage;
    private Color color;
    private string normalText = "ok na?";
    private string hoverText = "ok na!";

    private void Start()
    {
        buttonImage = GetComponent<Button>().image;
        color = buttonImage.color;

        color.a = 0f;
        buttonImage.color = color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        color.a = 1f;
        buttonImage.color = color;

        buttonText.text = hoverText;
        buttonText.fontSize = 60;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        color.a = 0f;
        buttonImage.color = color;

        buttonText.text = normalText;
        buttonText.fontSize = 50;
    }
}
