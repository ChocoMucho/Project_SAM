using UnityEngine;
using UnityEngine.UI;

public class UI_StartButton : UIBase
{
    [SerializeField] Button startButton;
    void Start()
    {
        // �̺�Ʈ 
        startButton.onClick.AddListener(() => SceneManagerExtend.Instance.LoadScene(Scene.MainGate));
    }

    void Update()
    {
        
    }
}
