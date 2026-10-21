using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; } = false;

    public static void SetPause(bool pause)
    {
        IsGamePaused = pause;
    }
}


// so far, pause controller only controls the items-related stuff (dragging, basket stuff)