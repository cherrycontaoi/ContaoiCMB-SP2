using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private Slider musicSlider;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);

            float savedVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            SetVolume(savedVol);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }

        musicSlider.SetValueWithoutNotify(audioSource.volume);
        musicSlider.onValueChanged.AddListener(SetVolume);

    }

    public void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if(audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                audioSource.Stop();
            }
            audioSource.Play();
        }
    }

    public static void SetVolume(float volume)
    {
        Instance.audioSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}
