using UnityEngine;
using UnityEngine.UI;

public abstract class GUIItemSlotBase : MonoBehaviour
{
    [SerializeField] protected Image imgIcon;
    // public ItemData itemData;
    
    // imgIcon = transform.Find("Img_Icon_Slot").GetComponent<Image>();
    
    public abstract void Initialization();

    public abstract void Show(Sprite icon, int pcs = 0, ItemData itemData = null);
    public abstract void Select();
}
