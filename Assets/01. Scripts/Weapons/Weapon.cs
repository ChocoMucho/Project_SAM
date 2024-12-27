using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform _muzzle;
    [SerializeField] GameObject _debugBullet;
    [SerializeField] LayerMask _layerMask = new LayerMask();
    [SerializeField] GameObject _impactEffect;
    [SerializeField] GameObject _muzzleEffect;

    private Player _player;
    private float _fireTimeout = 0.1f;
    private float _fireTimeoutDelta;
    private bool _isReloading;

    public int AmmoCapacity = 30;
    public int CurrentAmmo;

    private void Awake()
    {
        _player = transform.root.gameObject.GetComponent<Player>();
    }

    void Start()
    {
        CurrentAmmo = AmmoCapacity;
    }

    void Update()
    {
        _fireTimeoutDelta += Time.deltaTime;
    }

    public void Fire(Vector3 targetPos) // ¥‹º¯ πÊ«‚¿ª ¿ß«— ¡¬«•∏∏ πﬁ¿Ω
    {
        if(!CheckWeapon())
        {
            //_player.Animator.SetTrigger("Fire", false);
            return;
        }
        _player.Animator.SetTrigger(PlayerAnimatorHashes.Fire);
        _fireTimeoutDelta = 0f;

        if(Physics.Raycast(_muzzle.position, (targetPos - _muzzle.position).normalized, out RaycastHit hit, 1000))
        {
            Instantiate(_impactEffect, hit.point, Quaternion.identity);
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                target?.TakeDamage(_player.Damage);
            }
        }

        CurrentAmmo -= 1;

        Instantiate(_muzzleEffect, _muzzle.position, Quaternion.identity);
        Debug.Log($"º“√— «ˆ¿Á ¿‹≈∫: {CurrentAmmo}");
    }

    private bool CheckWeapon()
    {
        if (_fireTimeoutDelta < _fireTimeout) // Ω√∞£ √º≈©
        {
            return false;
        } 
        
        if(CurrentAmmo <= 0) // ¿‹≈∫ √º≈©
        {
            return false;
        }
            return true;
    }
}
