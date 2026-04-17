using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeekFlowScreenView : WeekFlowViewBase
{
    [SerializeField] private UI_CardView _cardPanel;

    private void Awake()
    {
        _cardPanel.OnCardOptionClicked += (cardDef, optionIndex) =>
        {
            RaiseCardOptionSelected(cardDef, optionIndex);
        };
    }

    public override void RenderSelections(IReadOnlyList<WeekSelectionEntryPresentation> presentations)
    {
        _cardPanel.SetCardData(presentations);
    }
}
