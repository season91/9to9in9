using System.Collections.Generic;
using UnityEngine;

public class UIStateGroup : MonoBehaviour
{
    [SerializeField] private RectTransform imgGaugeHealth;
    [SerializeField] private RectTransform imgGaugeStamina;
    [SerializeField] private RectTransform imgGaugeHunger;
    [SerializeField] private float maxGaugeWidth;

    Dictionary<StatType, RectTransform> gaugeStatType = new Dictionary<StatType, RectTransform>();
    
    private void Reset()
    {
        maxGaugeWidth = transform.Find("GUI_State_Health/Img_GaugeBG").GetComponent<RectTransform>().sizeDelta.x;
        imgGaugeHealth = transform.Find("GUI_State_Health/Img_Gauge")?.GetComponent<RectTransform>();
        imgGaugeStamina = transform.Find("GUI_State_Stamina/Img_Gauge")?.GetComponent<RectTransform>();
        imgGaugeHunger = transform.Find("GUI_State_Hunger/Img_Gauge")?.GetComponent<RectTransform>();
    }

    private void Awake()
    {
        gaugeStatType = new Dictionary<StatType, RectTransform>
        {
            [StatType.Health] = imgGaugeHealth,
            [StatType.Stamina] = imgGaugeStamina,
            [StatType.Hunger] = imgGaugeHunger
        };

        UIManager.Instance.OnUpdateStatUI += OnUpdateStateUI;
    }

    private void OnUpdateStateUI(StatType statType)
    {
        
        if (gaugeStatType.TryGetValue(statType, out var gauge))
        {
            float statRatio = CharacterManager.Player.statHandler.GetPercentage(statType);
            SetSizeGauge(gauge, statRatio);
        }
        else
        {
            MyDebug.Log($"this Stat type({statType}) is not UI State(Health, Hunger, Stamina)");
        }
    }

    void SetSizeGauge(RectTransform rectTr, float statRatio)
    {
        if (statRatio > 0)
        {
            if(!rectTr.transform.gameObject.activeSelf)
                rectTr.gameObject.SetActive(true);
            rectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, statRatio * maxGaugeWidth);
        }
        else
        {
            rectTr.gameObject.SetActive(false);
        }
    }
}
