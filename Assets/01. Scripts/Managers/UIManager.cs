using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, UIBase> _UIStorage = new Dictionary<string, UIBase>();

    protected override void Init()
    {
        _dontDestroyOnLoad = true;
        Clear();
        base.Init();
    }

    void Start()
    {
        
    }

    public void Regist(string UIName)
    {
        if(!_UIStorage.ContainsKey(UIName))
        {
            GameObject UIObj = ResourceManager.Instance.Instantiate($"UI/{UIName}");
            UIObj.name = UIName;
            _UIStorage.Add(UIName, UIObj.GetComponent<UIBase>());
        }
    }
      

    public void Remove(string UIName)
    {
        if (_UIStorage.ContainsKey(UIName))
            _UIStorage.Remove(UIName);
        else
            Assert.Fail($"UI load failed :{UIName} (Remove)");
    }

    public void Show(string UIName)
    {
        if(_UIStorage.ContainsKey(UIName))
            _UIStorage[UIName].ShowUI();
        else
            Assert.Fail($"UI load failed :{UIName} (Show)");
    }

    public void Hide(string UIName)
    {
        if (_UIStorage.ContainsKey(UIName))
            _UIStorage[UIName].HideUI();
        else
            Assert.Fail($"UI load failed :{UIName} (Hide)");
    }

    public void Clear() => _UIStorage.Clear();

}
