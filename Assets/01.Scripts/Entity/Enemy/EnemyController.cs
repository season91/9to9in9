using System;
using UnityEngine;
/// <summary>
/// 움직임을 적 종류마다 다르게 구현
/// </summary>
public class EnemyController : MonoBehaviour
{
    public IDamagable damagable;
    public IAttackAble attackable;
    public IMoveable moveable;

    private void Start()
    {
        damagable = GetComponent<IDamagable>();
        attackable = GetComponent<IAttackAble>();
        moveable = GetComponent<IMoveable>();
    }

    private void Update()
    {
        
    }
}
