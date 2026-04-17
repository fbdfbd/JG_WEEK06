using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStatusView : MonoBehaviour
{
    [System.Serializable]
    public class StatPanelUI
    {
        public TextMeshProUGUI StatNameText;
        public TextMeshProUGUI StatCntText;
        public Slider StatSlider;
    }

    [Header("Stat Panels")]
    [SerializeField] private StatPanelUI[] _statPanels;

    public void RenderStatus(ChildStatePresentation presentation)
    {
        IReadOnlyList<WeekStatPresentation> stats = presentation.Stats;

        int count = Mathf.Min(stats.Count, _statPanels.Length);

        for (int i = 0; i < count; i++)
        {
            WeekStatPresentation statData = stats[i];
            StatPanelUI panelUI = _statPanels[i];

            if (panelUI.StatNameText != null)
            {
                panelUI.StatNameText.text = statData.Label;
            }

            if (panelUI.StatCntText != null)
            {
                panelUI.StatCntText.text = statData.Value.ToString();
            }

            if (panelUI.StatSlider != null)
            {
                panelUI.StatSlider.value = statData.Value;
            }
        }

        // 데이터 개수보다 UI 패널이 더 많다면 비활성화 처리
        for (int i = count; i < _statPanels.Length; i++)
        {
            _statPanels[i].StatNameText.transform.parent.gameObject.SetActive(false);
        }
    }
}
