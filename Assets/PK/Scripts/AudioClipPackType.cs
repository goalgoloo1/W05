using System.Collections.Generic;
using System.Linq;
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

    public AudioClip GetClipsWithPrefix(string audioTagName)
    {
        // 접두사로 시작하는 모든 AudioTagObject 찾기
        List<AudioTagObject> found = audioClips.FindAll(tag => tag.audioTagName.StartsWith(audioTagName));
        
        if (found != null && found.Count > 0)
        {
            // 찾은 태그들의 클립을 리스트로 변환
            List<AudioClip> clips = found.Select(tag => tag.clipOfTag).ToList();
            return clips[Random.Range(0, clips.Count)];
        }

        Debug.LogWarning($"No audio clips with prefix '{audioTagName}' found in {name}.");
        return null;
    }
}