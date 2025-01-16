using UnityEngine;

public class UIBase : MonoBehaviour
{
    // UI���� �������� �κ��� ���.

    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false);

    public virtual void Init() { }
}
