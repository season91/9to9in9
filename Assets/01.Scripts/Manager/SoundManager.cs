using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SfxType
{
    Step, // 걷기
    Attack, // 공격함
    Hit, // 공격받음
    Mining, // 돌 캐기
    Logging, // 나무 캐기
    UIClick, // 클릭
    Dialogue // 대화음
}

[Serializable]
public class SfxClipGroup
{
    public SfxType type;
    public int clipCount;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Clips")]
    [SerializeField] private List<SfxClipGroup> sfxClipGroups = new();
    private Dictionary<SfxType, List<AudioClip>> sfxClipLists = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    
    public void InitSfx()
    {
        foreach (var group in sfxClipGroups)
        {
            LoadSfxList(group.type, group.clipCount);
        }
    }

    private void LoadSfxList(SfxType type, int count)
    {
        if (!sfxClipLists.ContainsKey(type))
            sfxClipLists[type] = new List<AudioClip>();

        var list = sfxClipLists[type];
        string prefix = type.ToString(); // StringNameSpace 대신 씀!!

        for (int i = 0; i < count; i++)
        {
            string address = $"{prefix}{i}";
            var clip = ResourceManager.Instance.GetResource<AudioClip>(address);
            if (clip != null)
                list.Add(clip);
            else
                Debug.LogWarning($"[SoundManager] {address} 클립을 찾을 수 없음");
        }
    }
    
    /// <summary>
    /// 사운드 재생
    /// </summary>
    /// <param name="type">사운드 타입 이름</param>
    public void PlayRandomSfx(SfxType type)
    {
        if (!sfxClipLists.TryGetValue(type, out var list) || list.Count == 0) return;

        AudioClip clip = (list.Count == 1)
            ? list[0]
            : list[Random.Range(0, list.Count)];
        
        sfxSource.PlayOneShot(clip);
        Debug.Log($"playing {clip.name}");
    }
}
