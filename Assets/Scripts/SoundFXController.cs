using UnityEngine;
using UnityEngine.UI;

public class SoundFXController : MonoBehaviour
{
    private static SoundFXController Instance;

    private static AudioSource audioSource;
    private static AudioSource randomPitchAudioSource;
    private static SoundFXLibrary soundFXLibrary;
    [SerializeField] private Slider sfxSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSource = audioSources[0];
            randomPitchAudioSource = audioSources[1];
            soundFXLibrary = GetComponent<SoundFXLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void Play(string soundName, bool randomPitch = false)
    {
        AudioClip audioClip = soundFXLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            if (randomPitch)
            {
                randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                randomPitchAudioSource.PlayOneShot(audioClip);
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
        randomPitchAudioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}
