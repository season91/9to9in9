using UnityEngine;

public class DamageableBehaviour : MonoBehaviour, IDamagable, IInitializable<BuildItemData>
{
    private BuildItemData data;
    private float currentHealth;
    
    public void Initialize(BuildItemData itemData)
    {
        data = itemData;
        currentHealth = data.health;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("item destroyed!");
        Destroy(this.gameObject);
    }
}