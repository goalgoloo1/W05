using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioTagObject
{
    public string audioTagName;
    public AudioClip clipOfTag;
}


[CreateAssetMenu(
    fileName = "SoundPack",
    menuName = "Scriptable Object/Sound Pack",
    order = int.MaxValue
)]
public class AudioClipPackType : ScriptableObject
{
    [Header("Sound Source")]
    public List<AudioTagObject> audioClips;

    // audioTagName에 맞는 AudioClip을 찾아 반환
    public AudioClip GetClip(string audioTagName)
    {
        AudioTagObject found = audioClips.Find(tag => tag.audioTagName == audioTagName);
        if (found != null)
        {
            return found.clipOfTag;
        }
        Debug.LogWarning($"Audio clip with tag '{audioTagName}' not found in {name}.");
        return null;
    }
}