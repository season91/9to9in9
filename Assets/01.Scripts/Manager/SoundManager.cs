using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    [SerializeField] private AudioSource bgmSource;
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Clips")]
    [SerializeField] private List<SfxClipGroup> sfxClipGroups = new();
    private Dictionary<SfxType, List<AudioClip>> sfxClipLists = new();

    [Header("Step")] 
    [SerializeField] private float stepInterval;
    private float stepTimer;
    
    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        ApplyVolumeDefaults();
    }

    private void Update()
    {
        ApplyVolume();
        
        audioMixer.GetFloat("MasterVolume", out float m);
        audioMixer.GetFloat("BGMVolume", out float b);
        audioMixer.GetFloat("SFXVolume", out float s);
        
        Debug.Log($"[Mixer dB] Master: {m}, BGM: {b}, SFX: {s}");
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
            if (clip)
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
    }

    public void PlayStepSfx()
    {
        stepTimer += Time.deltaTime;
        if (!(stepTimer >= stepInterval)) return;
        
        stepTimer = 0f;
        PlayRandomSfx(SfxType.Step);
    }

    public void PlayParticle(ParticleSystem particle)
    {
        if (particle == null)  return;
        
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // 초기화
        particle.Play();
    }
    
    private void ApplyVolumeDefaults()
    {
        SetMasterVolume(1f);
        SetBgmVolume(1f);
        SetSfxVolume(1f);
    }
    
    private void ApplyVolume()
    {
        float masterDb = Mathf.Log10(Mathf.Clamp(masterVolume, 0.0001f, 1f)) * 20f;
        float bgmDb = Mathf.Log10(Mathf.Clamp(bgmVolume, 0.0001f, 1f)) * 20f;
        float sfxDb = Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20f;

        audioMixer.SetFloat("MasterVolume", masterDb);
        audioMixer.SetFloat("BGMVolume", bgmDb);
        audioMixer.SetFloat("SFXVolume", sfxDb);
    }
    
    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
    }

    public void SetBgmVolume(float value)
    {
        SetVolume("BGMVolume", value);
    }

    public void SetSfxVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }
    
    private void SetVolume(string exposedParam, float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
    }

    
}