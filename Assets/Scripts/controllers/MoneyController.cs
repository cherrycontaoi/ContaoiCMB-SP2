using UnityEngine;
using UnityEngine.EventSystems;

public class MoneyController : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (PauseController.IsGamePaused) return;
        CounterDisplayController.Instance.GetMoney();
    }
}