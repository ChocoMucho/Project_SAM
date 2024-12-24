using UnityEngine;

public class MainScene : BaseScene
{
    GameManager gameManager;
    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
