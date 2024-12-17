using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum GateState
{
    Opening,
    Closing,
    Waiting,
}

public class Gate : MonoBehaviour
{
    float moveSpeed = 0.5f;
    Vector3 InitPosition;
    Vector3 targetPosition;
    void Start()
    {
        InitPosition = transform.position;
        targetPosition = transform.position + new Vector3(0,7f,0);
    }

    public void OpenTheGate()
    {
        StartCoroutine(Open());
    }

    public void CloseTheGate()
    {
        StartCoroutine(Close());
    }

    IEnumerator Open()
    {
        while(Vector3.Distance(transform.position, targetPosition) >= 0.1f)
        {
            transform.position += Vector3.up * Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.position = targetPosition;
    }

    IEnumerator Close()
    {
        while (Vector3.Distance(transform.position, InitPosition) >= 0.1f)
        {
            transform.position += Vector3.down * Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.position = InitPosition;
    }
}
