using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float time = 0f;
    [SerializeField] private float fullDayLength = 300;
    [SerializeField] private float startTime = 0.4f;
    [SerializeField] private float timeRate;
    [SerializeField] private Vector3 noon;
    private float prevTime;

    [Header("Sun")]
    [SerializeField] private Light sun;
    [SerializeField] private Gradient sunColor;
    [SerializeField] private AnimationCurve sunIntensity;

    [Header("Moon")]
    [SerializeField] private Light moon;
    [SerializeField] private Gradient moonColor;
    [SerializeField] private AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    [SerializeField] private AnimationCurve lightingIntensityMultiplier;
    [SerializeField] private AnimationCurve reflectionIntensityMultiplier;

    
    [Header("Sky Box")]
    [SerializeField] private Material daySkyBox;
    [SerializeField] private Material nightSkyBox;
    [SerializeField] private Material blendSkyBox;
    
    public bool isDay = true;
    public int dayCount = 0;
    
    // 기능 구현 후 리팩토링 필요
    public bool isSpawnedResource = false;
    public bool isSpawnedEnemy = false;
    
    
    [SerializeField] private float intensityChangeSpeed = 1f;
    
    // GameManager에서 델리게이트에 함수 등록해주는 로직 구현
    public static event Action OnDayStarted;
    public static event Action OnNightStarted;
    
    private void Awake()
    {
        sun = transform.Find("Sun").GetComponent<Light>();
        if(sun == null) {Debug.Log("Sun 자식에서 찾을 수 없음");}
        moon = transform.Find("Moon").GetComponent<Light>();
        if(moon == null){Debug.Log("Moon 자식에서 찾을 수 없음");}
    }

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
        noon = new Vector3(90f, 0f, 0f);
        dayCount = 0;
    }

    private void Update()
    {
        prevTime = time;
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        if (time < prevTime && !isDay)
        {
            ++dayCount;
            isSpawnedResource = false;
            isSpawnedEnemy = false;
            Debug.Log(dayCount);
        }
        
        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        bool dayTimeSkyBox = (time >= 0.35f && time <= 0.65f);
        bool nightTimeSkyBox = (time < 0.15f || time > 0.85f);
        Material targetSkyBox;

        if (dayTimeSkyBox)
        {
            targetSkyBox = daySkyBox;
        }else if (nightTimeSkyBox)
        {
            targetSkyBox = nightSkyBox;
        }
        else
        {
            targetSkyBox = blendSkyBox;
        }
        
        if (RenderSettings.skybox != targetSkyBox)
        {
            RenderSettings.skybox = targetSkyBox;
            DynamicGI.UpdateEnvironment();
        }
        
        if (sun.gameObject.activeInHierarchy)
        {
            isDay = true;
            if (!isSpawnedResource)
            {
                OnDayStarted?.Invoke();
                isSpawnedResource = true;
            }
        }
        else
        {
            isDay = false;
            if (!isSpawnedEnemy)
            {
                OnNightStarted?.Invoke();
                isSpawnedEnemy = true;
            }
        }
        
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);

        lightSource.transform.eulerAngles = noon * ((time - (lightSource == sun ? 0.25f : 0.75f)) * 4.0f);
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;
        //lightSource.intensity = Math.Clamp(lightSource.intensity, intensity, Time.deltaTime);

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
    
    void SetDaySkyBox()
    {
        RenderSettings.skybox = daySkyBox;
        DynamicGI.UpdateEnvironment(); 
    }
    
    void SetNightSkyBox()
    {
        RenderSettings.skybox = nightSkyBox;
        DynamicGI.UpdateEnvironment(); 
    }
}
