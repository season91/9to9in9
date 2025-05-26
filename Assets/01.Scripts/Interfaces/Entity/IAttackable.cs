using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 공격하는 애들
/// </summary>
public interface IAttackAble
{
    public float AttackPower { get; }
    public void Attack();
}

