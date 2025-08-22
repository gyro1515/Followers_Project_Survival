using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            // 씬에 없더라도 생성되도록
            if (instance == null)
            {
                var singletonGO = new GameObject($"{typeof(T)}");
                instance = singletonGO.AddComponent<T>(); // Awake() 실행
            }
            return instance;
        }
    }

    protected virtual void Awake() // 씬에 올려 놓았다면 바로 실행 될 것
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else if (instance != this)
        {
            // 현재는 씬 하나라 중복 생성될 일은 없겠지만, 혹시 모르니까
            Debug.LogWarning($"중복된 {typeof(T).Name} 싱글톤이 발견되어 파괴됩니다.");
            Destroy(gameObject); // 중복 제거
        }
    }

    public virtual void Release() // 추천 받은 기능, 씬 전환 시 해당 매니저를 파괴할 때도 있을테니까
    {
        if (instance == null) return;
        if (instance.gameObject) Destroy(instance.gameObject);

        instance = null;
    }
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
