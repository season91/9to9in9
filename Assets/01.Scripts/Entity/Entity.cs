using UnityEngine;
/// <summary>
/// [공통스탯]
/// </summary>
public abstract class Entity : MonoBehaviour, IDamagable
{
    public StatHandler statHandler = new StatHandler();

    public virtual void TakeDamage(float damage)
    {
        statHandler.Modify(StatType.Health, -damage);
        if (statHandler.IsEmpty(StatType.Health))
            Die();
    }

    public abstract void Die();
}