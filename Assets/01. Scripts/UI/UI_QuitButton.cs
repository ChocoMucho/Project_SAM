using UnityEngine;
using UnityEngine.UI;

public class UI_QuitButton : UIBase
{
    [SerializeField] Button quitButton;
    void Start()
    {
        // �̺�Ʈ
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    void Update()
    {
        
    }
}
