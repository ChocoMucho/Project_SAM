using UnityEngine;
using UnityEngine.UI;

public class UI_QuitButton : UIBase
{
    [SerializeField] Button quitButton;
    void Start()
    {
        // ÀÌº¥Æ®
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    void Update()
    {
        
    }
}
