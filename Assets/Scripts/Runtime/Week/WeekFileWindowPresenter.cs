using System;
using System.Collections.Generic;
using System.Linq;

public sealed class WeekFileWindowPresenter
{
    private readonly WeekFileWindowView _view;
    private readonly Func<SO_CardInfoDefinition, int> _getSelectionIndex;
    private readonly Action<SO_CardInfoDefinition, int> _setSelectionIndex;

    private readonly List<WeekFileCategoryGroup> _groups = new();
    private int _currentCategoryIndex;

    public WeekFileWindowPresenter(
        WeekFileWindowView view,
        Func<SO_CardInfoDefinition, int> getSelectionIndex,
        Action<SO_CardInfoDefinition, int> setSelectionIndex)
    {
        _view = view;
        _getSelectionIndex = getSelectionIndex;
        _setSelectionIndex = setSelectionIndex;

        _view.OpenRequested += HandleOpenRequested;
        _view.CloseRequested += HandleCloseRequested;
        _view.PreviousRequested += HandlePreviousRequested;
        _view.NextRequested += HandleNextRequested;
    }

    public void SetWeek(IReadOnlyList<WeekCardEntryData> entries)
    {
        _groups.Clear();

        foreach (WeekCardEntryData entry in entries)
        {
            if (entry?.Card == null)
            {
                continue;
            }

            WeekFileCategoryGroup group = _groups.FirstOrDefault(existing => existing.CardType == entry.Card.CardType);
            if (group == null)
            {
                group = new WeekFileCategoryGroup(entry.Card.CardType);
                _groups.Add(group);
            }

            group.Cards.Add(entry.Card);
        }

        _currentCategoryIndex = Clamp(_currentCategoryIndex, 0, Math.Max(0, _groups.Count - 1));
        _view.SetWindowVisible(false, false);
        Refresh();
    }

    public void Refresh()
    {
        if (_groups.Count == 0)
        {
            _view.ShowEmptyState("이번 주에 확인할 정보 파일이 없습니다.");
            return;
        }

        WeekFileCategoryGroup currentGroup = _groups[_currentCategoryIndex];
        List<WeekFileCardPresentation> cards = currentGroup.Cards
            .Select(BuildCardPresentation)
            .ToList();

        WeekFileCategoryPagePresentation presentation = new(
            $"카테고리 {_currentCategoryIndex + 1} / {_groups.Count}",
            currentGroup.CardType != null ? currentGroup.CardType.DisplayName : "분류 없음",
            $"이번 분류 {currentGroup.Cards.Count}건",
            cards,
            _currentCategoryIndex > 0,
            _currentCategoryIndex < _groups.Count - 1);

        _view.RenderCategory(presentation, HandleSelectOption);
    }

    private WeekFileCardPresentation BuildCardPresentation(SO_CardInfoDefinition cardDefinition)
    {
        int selectedOptionIndex = _getSelectionIndex(cardDefinition);
        CardOptionData selectedOption = GetSelectedOption(cardDefinition, selectedOptionIndex);

        return new WeekFileCardPresentation(
            cardDefinition,
            cardDefinition.Title,
            cardDefinition.CardType != null ? cardDefinition.CardType.DisplayName : "정보 카드",
            cardDefinition.OriginalText,
            selectedOption != null && !string.IsNullOrWhiteSpace(selectedOption.Label)
                ? selectedOption.Label
                : "선택 대기",
            selectedOptionIndex,
            cardDefinition.Options ?? Array.Empty<CardOptionData>());
    }

    private void HandleOpenRequested()
    {
        if (_groups.Count == 0)
        {
            return;
        }

        _view.SetWindowVisible(true, true);
        Refresh();
    }

    private void HandleCloseRequested()
    {
        _view.SetWindowVisible(false, true);
    }

    private void HandlePreviousRequested()
    {
        if (_currentCategoryIndex <= 0)
        {
            return;
        }

        _currentCategoryIndex--;
        Refresh();
    }

    private void HandleNextRequested()
    {
        if (_currentCategoryIndex >= _groups.Count - 1)
        {
            return;
        }

        _currentCategoryIndex++;
        Refresh();
    }

    private void HandleSelectOption(SO_CardInfoDefinition cardDefinition, int optionIndex)
    {
        _setSelectionIndex(cardDefinition, optionIndex);
        Refresh();
    }

    private static CardOptionData GetSelectedOption(SO_CardInfoDefinition cardDefinition, int selectedOptionIndex)
    {
        if (cardDefinition?.Options == null || cardDefinition.Options.Length == 0)
        {
            return null;
        }

        int clampedIndex = Clamp(selectedOptionIndex, 0, cardDefinition.Options.Length - 1);
        return cardDefinition.Options[clampedIndex];
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}

public sealed class WeekFileCategoryGroup
{
    public WeekFileCategoryGroup(SO_CardInfoTypeDefinition cardType)
    {
        CardType = cardType;
        Cards = new List<SO_CardInfoDefinition>();
    }

    public SO_CardInfoTypeDefinition CardType { get; }
    public List<SO_CardInfoDefinition> Cards { get; }
}

public readonly struct WeekFileCategoryPagePresentation
{
    public WeekFileCategoryPagePresentation(
        string categoryIndexLabel,
        string categoryName,
        string categoryCountLabel,
        IReadOnlyList<WeekFileCardPresentation> cards,
        bool canMovePrevious,
        bool canMoveNext)
    {
        CategoryIndexLabel = categoryIndexLabel;
        CategoryName = categoryName;
        CategoryCountLabel = categoryCountLabel;
        Cards = cards;
        CanMovePrevious = canMovePrevious;
        CanMoveNext = canMoveNext;
    }

    public string CategoryIndexLabel { get; }
    public string CategoryName { get; }
    public string CategoryCountLabel { get; }
    public IReadOnlyList<WeekFileCardPresentation> Cards { get; }
    public bool CanMovePrevious { get; }
    public bool CanMoveNext { get; }
}

public readonly struct WeekFileCardPresentation
{
    public WeekFileCardPresentation(
        SO_CardInfoDefinition cardDefinition,
        string title,
        string typeName,
        string originalText,
        string selectedOptionLabel,
        int selectedOptionIndex,
        IReadOnlyList<CardOptionData> options)
    {
        CardDefinition = cardDefinition;
        Title = title;
        TypeName = typeName;
        OriginalText = originalText;
        SelectedOptionLabel = selectedOptionLabel;
        SelectedOptionIndex = selectedOptionIndex;
        Options = options;
    }

    public SO_CardInfoDefinition CardDefinition { get; }
    public string Title { get; }
    public string TypeName { get; }
    public string OriginalText { get; }
    public string SelectedOptionLabel { get; }
    public int SelectedOptionIndex { get; }
    public IReadOnlyList<CardOptionData> Options { get; }
}
