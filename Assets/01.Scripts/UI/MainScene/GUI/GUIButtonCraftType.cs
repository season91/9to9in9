using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIButtonCraftType : MonoBehaviour
{
    [SerializeField] private Button btnCraftType;
    [SerializeField] private TextMeshProUGUI tmpBtn;
    // [SerializeField] private EquipType craftType;
    private string craftType;

    // public bool IsSameType(string strCraftType) => craftType == strCraftType;
    
    void Reset()
    {
        btnCraftType = GetComponent<Button>();
        tmpBtn = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void Initialization()
    {
        gameObject.SetActive(false);
    }

    public void Setting(string strCraftType)
    {
        gameObject.SetActive(true);
        
        craftType = strCraftType;
        tmpBtn.text = craftType;
    }
    
    public void SetClickEvent(UnityAction<string> callback)
    {
        btnCraftType.onClick.RemoveAllListeners();
        btnCraftType.onClick.AddListener(() => callback(craftType));
    }

    public string GetCraftType() => craftType;
}
