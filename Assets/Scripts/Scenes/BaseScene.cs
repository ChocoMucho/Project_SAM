using UnityEngine;
using UnityEngine.EventSystems;

public enum Scene
{
    Unknown,
    Lobby,
    Battle,
}

public abstract class BaseScene : MonoBehaviour
{
    public Scene SceneType { get; protected set; } = Scene.Unknown;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (null == GameObject.FindObjectOfType(typeof(EventSystem)))
            SetEventSystem();
    }

    public abstract void Clear();

    private void SetEventSystem()
    {
        GameObject eventSystem = new GameObject("@EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
}
