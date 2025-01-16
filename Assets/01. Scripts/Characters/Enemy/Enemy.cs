using UnityEngine;

public class Enemy : Entity
{
    public GameObject Target;
    //=====Behavior Tree=====
    protected BehaviorTree Tree;

    public virtual void InitTree() { }
    public override void TakeDamage(int damage){}
}
