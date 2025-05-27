using UnityEngine;
/// <summary>
/// [공통스탯]
/// </summary>
public abstract class Entity : MonoBehaviour
{
    protected float health;
    protected float maxHealth;

    public virtual void ChangeHealth(float value)
    {
        health = Mathf.Min(Mathf.Max(health + value, 0), maxHealth);
    }
}
