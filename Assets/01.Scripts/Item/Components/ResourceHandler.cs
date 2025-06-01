using UnityEngine;

public enum RootResourceType
{
   Rock,
   Tree
}

public class ResourceHandler : MonoBehaviour
{
   [SerializeField] private ItemData itemToGive;
   [SerializeField] private int quantityPerHit;
   [SerializeField] private float capacity;
   [SerializeField] private ParticleSystem effect;
   
   [SerializeField] private RootResourceType rootResourceType;
   
   public void Gather(Vector3 hitPoint, Vector3 hitNormal)
   {
      SetSound();
      
      for(int i = 0; i < quantityPerHit; i++)
      {
         if (capacity <= 0) break;

         capacity -= 1;
         Debug.Log($"{this.name} 캐는 중 - 남은 체력 {capacity}");
         SoundManager.Instance.PlayParticle(effect);
         
      }

      if (!(capacity <= 0)) return;
      
      SpawnManager.Instance.ItemSpawnInPosition(itemToGive.name, transform.position);

      SpawnManager.Instance.ReleaseObject(itemToGive.itemCode, gameObject);
   }

   private void SetSound()
   {
      switch (rootResourceType)
      {
         case RootResourceType.Rock:
            SoundManager.Instance.PlayRandomSfx(SfxType.Mining);
            break;
         
         case RootResourceType.Tree:
            SoundManager.Instance.PlayRandomSfx(SfxType.Logging);
            break;
      }
   }
}