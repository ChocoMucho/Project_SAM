using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Player Player;

    protected override void Init()
    {
        _dontDestroyOnLoad = true;
        base.Init();
    }
    void Start()
    {
    }

    void Update()
    {
        
    }
}
