using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Init()
    {
        _dontDestroyOnLoad = true;
        base.Init();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene("MainGate");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
