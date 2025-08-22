using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureUI : MonoBehaviour
{
    [Header("온도 UI 설정")]
    [SerializeField] RectTransform needleRectTransform;

    public void SetTemperature(float value)
    {
        // 0~1 범위를 0~2로 확장
        float t = value * 2f;
        // pingpong = 0일 때 -120도, 0.5일 때 0도, 1일 때 120도
        float angle = Mathf.Lerp(120f, -120f, Mathf.PingPong(t, 1f));
        needleRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
