using UnityEngine;

[CreateAssetMenu(fileName = "ShootConfigSO", menuName = "Guns/ShootConfigSO", order = 2)]
public class ShootConfigSO : ScriptableObject
{
    public LayerMask HitMask;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;
}
