using UnityEngine;

public class CursorController : MonoBehaviour
{

    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    public void SetToEraser()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void SetToDefault()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

}
