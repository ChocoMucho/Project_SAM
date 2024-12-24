using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected bool _dontDestroyOnLoad = false;
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null) // �� ������ ã��
#if unity_2023_1_OR_NEWER
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Exclude);
#else
                _instance = FindObjectOfType<T>();
#endif
            if (_instance == null) // �����Ƿ� ����
            {
                GameObject @object = new GameObject(typeof(T).Name);
                _instance = @object.AddComponent<T>();
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Init();
    }

    protected virtual void Init()
    {
        if (_dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
}
