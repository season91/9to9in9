using UnityEngine;
/// <summary>
/// 공격시 - 원거리, 근거리로 나눈다면 근거리 적, 보스, 각각 Enemy 상속 받아서 사용
/// IDamageble, IMoveable, IAttackable
/// </summary>
public abstract class Enemy : Entity, IDamagable, IMoveable
{
    public abstract void TakeDamage();

    public abstract void Move();
}
