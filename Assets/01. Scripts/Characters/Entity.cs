using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamageable
{
    //=====Status=====
    [SerializeField] StatusSO statusSO;
    public int MaxHP { get; protected set; }
    public int CurrentHP { get; protected set; }
    public int Damage { get; protected set; }
    public bool IsDead { get; protected set; }
    public event Action OnDeath;

    public abstract void TakeDamage(int damage);

    public virtual void InitStatus()
    {
        MaxHP = statusSO.HP;
        CurrentHP = statusSO.HP;
        Damage = statusSO.Damage;
        IsDead = false;
    }

    protected void TriggerOnDeath() => OnDeath?.Invoke();
}
