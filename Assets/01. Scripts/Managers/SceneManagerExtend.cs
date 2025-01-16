using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerExtend : Singleton<SceneManagerExtend>
{
    protected override void Init()
    {
        _dontDestroyOnLoad = true;
        base.Init();
    }

    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Scene scene)
    {
        CurrentScene.Clear();
        SceneManager.LoadScene(scene.ToString());
    }
}
