using UnityEngine;

public interface IStrategy
{
    public Status Process();
    public void Reset() { }
}
