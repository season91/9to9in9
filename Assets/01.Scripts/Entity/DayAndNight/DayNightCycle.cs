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

    public bool isDay = true;
    
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
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        if (sun.gameObject.activeInHierarchy)
        {
            isDay = true;
        }
        else
        {
            isDay = false;
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

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
