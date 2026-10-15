using UnityEngine;
using UnityEngine.UI;

public class SoundFXController : MonoBehaviour
{
    private static SoundFXController Instance;

    private static AudioSource audioSource;
    private static AudioSource randomPitchAudioSource;
    private static SoundFXLibrary soundFXLibrary;
    [SerializeField] private Slider sfxSlider;

    private const string SFX_VOLUME_KEY = "SFXVolume";

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

            float savedVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            SetVolume(savedVol);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sfxSlider.SetValueWithoutNotify(audioSource.volume);
        sfxSlider.onValueChanged.AddListener(OnVolumeChanged);
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

    private void OnVolumeChanged(float volume)
    {
        SetVolume(volume);

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}
