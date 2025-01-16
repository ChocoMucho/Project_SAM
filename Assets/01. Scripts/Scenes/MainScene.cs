using UnityEngine;

public class MainScene : BaseScene
{
    GameManager gameManager;
    public override void Clear()
    {
        UIManager.Instance.Clear();
    }

    protected override void Init()
    {
        base.Init();
        gameManager = GameManager.Instance;
        SceneType = Scene.Main;

        // UI
        UIManager.Instance.Regist("UI_StartButton");
        UIManager.Instance.Show("UI_StartButton");
        UIManager.Instance.Regist("UI_QuitButton");
        UIManager.Instance.Show("UI_QuitButton");
    }


    void Update()
    {
        
    }
}
