using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "GunSO", menuName = "Guns/GunSO", order = 0)]
public class GunSO : ScriptableObject
{
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public ShootConfigSO ShootConfig;
    public TrailConfigSO TrailConfig;

    private MonoBehaviour ActiveMonoBehavior;
    private GameObject Model;
    private float LastShootTime;
    private ParticleSystem ShootSystem;
    private ObjectPool<TrailRenderer> TrailPool;
}
