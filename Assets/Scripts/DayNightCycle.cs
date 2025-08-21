using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayNightCycle : MonoBehaviour
{
    enum ETemperatureState
    {
        None,
        Normal,
        Hot,
        Cold
    }
    ETemperatureState temState = ETemperatureState.None;
    // 하루 시간을 0 ~ 1로 치환
    // 길이를 늘리고 싶으면 fullDayLength를 조절
    [Range(0.0f, 1.0f)]
    [SerializeField] float time;
    [SerializeField] float fullDayLength;
    [SerializeField] float startTime = 0.4f;
    [SerializeField] Vector3 noon;
    public float DayTime 
    { 
        get { return time; } 
        set 
        { 
            time = value; 
            DayTimeChanged?.Invoke(time);
            if(time >= 0.45f && time <= 0.55f) // 가장 더울때는 갈증 추가 감소
            {
                if (temState == ETemperatureState.Hot) return;
                Debug.Log("더움");
                temState = ETemperatureState.Hot;
                OnDayTimeStatDrain?.Invoke(StatType.Thirst, 0.1f);
            }
            else if (time <= 0.05f || time >= 0.95f) // 가중 추울때는 굶주림 추가 감소
            {
                if (temState == ETemperatureState.Cold) return;
                Debug.Log("추움");
                temState = ETemperatureState.Cold;
                OnDayTimeStatDrain?.Invoke(StatType.Hunger, 0.15f);
            }
            else // 다른 시간은 원래대로 감소
            {
                if (temState == ETemperatureState.Normal) return;
                temState = ETemperatureState.Normal;
                Debug.Log("적당");
                OnDayTimeStatDrain?.Invoke(StatType.Hunger, 0);
                OnDayTimeStatDrain?.Invoke(StatType.Thirst, 0);
            }
        } 
    }
    public event Action<float> DayTimeChanged;
    public event Action<StatType, float> OnDayTimeStatDrain;

    private float timeRate;
    public float GetTime { get { return time; } }

    [Header("Sun")]
    [SerializeField] Light sun;
    [SerializeField] Gradient sunColor;
    [SerializeField] AnimationCurve sunIntensity;
    public Light Sun { get { return sun; } }

    [Header("Moon")]
    [SerializeField] Light moon;
    [SerializeField] Gradient moonColor;
    [SerializeField] AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    [SerializeField] AnimationCurve lightingIntensityMultiplier;
    [SerializeField] AnimationCurve reflectionIntensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        DayTime = (time + timeRate * Time.deltaTime) % 1.0f;
        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        // Evaluate는 Inspector에 그린 그래프에서 time을 입력받으면 특정 값을 return
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
        // 온도 UI 설정
        // UIManager.Instance.SetTemperatureUI(time); // 델리게이트로 변경
        // 여기서 추가로 온도(시간)에 따라 플레이어에게 데미지 주기?
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);

        // 하루 시간(0 ~ 1)과 해/달의 자전주기(0 ~ 360)의 값을 동기화.
        // 해와 달은 180도 차이가 항상 나기 때문에 0.5f(180/360)의 차이
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
