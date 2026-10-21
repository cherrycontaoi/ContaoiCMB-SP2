using System.Collections.Generic;
using UnityEngine;

public class SoundFXLibrary : MonoBehaviour
{
    // key -> name
    [SerializeField] private SoundFXGroup[] soundFXGroups;
    private Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        InitializeDictionary();    
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        
        foreach(SoundFXGroup soundFXGroup in soundFXGroups)
        {
            soundDictionary[soundFXGroup.name] = soundFXGroup.audioClips;
        }
    }
    
    public AudioClip GetRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[Random.Range(0, audioClips.Count)];
            }
        }

        return null;
    }

}

[System.Serializable]
public struct SoundFXGroup
{
    public string name; //key
    public List<AudioClip> audioClips;

}
