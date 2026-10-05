using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public TMP_Text buttonText;
    private string normalText = "ok na?";
    private string hoverText = "ok na!";
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.text = hoverText;
        buttonText.fontSize = 60;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.text = normalText;
        buttonText.fontSize = 50;
    }
}
