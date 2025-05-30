using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
   [SerializeField] private ItemData itemToGive;
   [SerializeField] private int quantityPerHit;
   [SerializeField] private float capacity;
   
   public void Gather(Vector3 hitPoint, Vector3 hitNormal)
   {
      for(int i = 0; i < quantityPerHit; i++)
      {
         if (capacity <= 0) break;

         capacity -= 1;
         Instantiate(itemToGive.prefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
      }

      if(capacity <= 0)
      {
         Destroy(gameObject);
      }
   }
}