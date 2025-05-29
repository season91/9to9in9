using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamagable, IInitializable<BuildItemData>
{
    private BuildItemData data;
    
    public void Initialize(BuildItemData itemData)
    {
        data = itemData;
    }
    
    public void TakeDamage(float damage)
    {
        data.health -= damage;
        if (data.health <= 0)
        {
            data.health = 0;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("item destroyed!");
        Destroy(this.gameObject);
    }
}