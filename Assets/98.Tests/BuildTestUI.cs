using UnityEngine;

public class BuildTestUI : MonoBehaviour
{
    [SerializeField] private BuildItemData wallItem;
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BuildManager.Instance.EnterBuildMode(wallItem);
        }
    }
}
