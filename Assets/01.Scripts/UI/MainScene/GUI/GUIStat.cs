using TMPro;
using UnityEngine;

public class GUIStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpStatTitle;
    [SerializeField] private TextMeshProUGUI tmpStatValue;

    private void OnDrawGizmosSelected()
    {
        tmpStatTitle = transform.Find("Tmp_StatTitle")?.GetComponentInChildren<TextMeshProUGUI>();
        tmpStatValue = transform.Find("Tmp_StatValue")?.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Initialization()
    {
        gameObject.SetActive(false);
    }
    
    public void Show(string title, string value)
    {
        gameObject.SetActive(true);
        tmpStatTitle.text = title;
        tmpStatValue.text = value;
    }
}
