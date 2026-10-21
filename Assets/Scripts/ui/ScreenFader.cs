using System.Threading.Tasks;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha, t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await Task.Yield();
        }

        canvasGroup.alpha = targetTransparency;
    }

    public void StartFaded()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public async Task FadeOut()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        await Fade(1);  // fade to black
    }

    public async Task FadeIn()
    {
        await Fade(0); // fade to transparent
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);
    }


}
