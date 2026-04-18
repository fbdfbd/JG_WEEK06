using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeekFlowScreenView : WeekFlowViewBase
{
    [SerializeField] private UI_CardView _cardPanel;
    [SerializeField] private UI_CharacterStatusView _characterStatusPanel;
    [SerializeField] private UI_TopView _topPanel;
    [SerializeField] private UI_BottomView _bottomPanel;
    [SerializeField] private UI_SemanticView _semanticPanel;

    private bool _isInfoPanelVisible = true;

    private void Awake()
    {
        _cardPanel.OnCardOptionClicked += (cardDef, optionIndex) =>
        {
            RaiseCardOptionSelected(cardDef, optionIndex);
        };

        if (_bottomPanel != null)
        {
            _bottomPanel.OnInfoButtonClicked += HandleInfoButtonClicked;
            _bottomPanel.OnNextDayButtonClicked += HandleNextDayButtonClicked;
        }
    }

    private void OnDestroy()
    {
        if (_bottomPanel != null)
        {
            _bottomPanel.OnInfoButtonClicked -= HandleInfoButtonClicked;
            _bottomPanel.OnNextDayButtonClicked -= HandleNextDayButtonClicked;
        }
    }

    private void HandleInfoButtonClicked()
    {
        _isInfoPanelVisible = !_isInfoPanelVisible;

        if (_cardPanel != null) _cardPanel.gameObject.SetActive(_isInfoPanelVisible);
        if (_semanticPanel != null) _semanticPanel.gameObject.SetActive(_isInfoPanelVisible);
    }

    private void HandleNextDayButtonClicked()
    {
        RaiseRunWeekRequested();
    }

    public override void RenderSelectionGroups(IReadOnlyList<WeekSelectionCategoryGroupPresentation> groups)
    {
        _cardPanel.SetCardGroups(groups);
    }

    public override void RenderChildState(ChildStatePresentation presentation)
    {
        _characterStatusPanel.RenderStatus(presentation);
    }

    public override void RenderWeekHeader(WeekHeaderPresentation presentation)
    {
        // 뷰 스스로 데이터를 해석하지 않고, 하위 패널에 그대로 전달
        if (_topPanel != null)
        {
            _topPanel.RenderHeader(presentation);
        }
    }
}
