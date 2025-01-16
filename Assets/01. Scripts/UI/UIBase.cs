using UnityEngine;

public class UIBase : MonoBehaviour
{
    // UI들의 공통적인 부분을 담당.

    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false);

    public virtual void Init() { }
}
