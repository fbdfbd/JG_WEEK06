using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeekFlowScreenView : MonoBehaviour
{
    [SerializeField] private UI_CardView _cardPanel;
    [SerializeField] private UI_CharacterStatusView _characterStatusPanel;
    [SerializeField] private UI_TopView _topPanel;
    [SerializeField] private UI_BottomView _bottomPanel;

    public event Action RunWeekRequested;
    public event Action<SO_CardInfoDefinition, int> CardOptionSelected;

    private bool _isInfoPanelVisible = true;

    private void Awake()
    {
        BindCardPanelEvents();
        BindBottomPanelEvents();
    }

    private void OnDestroy()
    {
        UnbindCardPanelEvents();
        UnbindBottomPanelEvents();
    }

    public void RenderSelectionGroups(IReadOnlyList<WeekSelectionCategoryGroupPresentation> groups)
    {
        if (_cardPanel == null)
        {
            return;
        }

        _cardPanel.SetCardGroups(groups);
    }

    public void RenderChildState(ChildStatePresentation presentation)
    {
        if (_characterStatusPanel == null)
        {
            return;
        }

        _characterStatusPanel.RenderStatus(presentation);
    }

    public void RenderWeekHeader(WeekHeaderPresentation presentation)
    {
        if (_topPanel == null)
        {
            return;
        }

        _topPanel.RenderHeader(presentation);
    }

    private void BindCardPanelEvents()
    {
        if (_cardPanel == null)
        {
            return;
        }

        _cardPanel.OnCardOptionClicked += HandleCardOptionClicked;
    }

    private void UnbindCardPanelEvents()
    {
        if (_cardPanel == null)
        {
            return;
        }

        _cardPanel.OnCardOptionClicked -= HandleCardOptionClicked;
    }

    private void BindBottomPanelEvents()
    {
        if (_bottomPanel == null)
        {
            return;
        }

        _bottomPanel.OnInfoButtonClicked += HandleInfoButtonClicked;
        _bottomPanel.OnNextDayButtonClicked += HandleNextDayButtonClicked;
    }

    private void UnbindBottomPanelEvents()
    {
        if (_bottomPanel == null)
        {
            return;
        }

        _bottomPanel.OnInfoButtonClicked -= HandleInfoButtonClicked;
        _bottomPanel.OnNextDayButtonClicked -= HandleNextDayButtonClicked;
    }

    private void HandleCardOptionClicked(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        CardOptionSelected?.Invoke(cardDefinition, optionIndex);
    }

    private void HandleInfoButtonClicked()
    {
        _isInfoPanelVisible = !_isInfoPanelVisible;

        if (_cardPanel != null)
        {
            _cardPanel.gameObject.SetActive(_isInfoPanelVisible);
        }
    }

    private void HandleNextDayButtonClicked()
    {
        RunWeekRequested?.Invoke();
    }
}
