using NUnit.Framework;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    protected override void Init()
    {
        _dontDestroyOnLoad = true;
        base.Init();
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Assert.Fail("path can't be null");
            return null;
        }

        return Instantiate(prefab, parent);
    }
}
