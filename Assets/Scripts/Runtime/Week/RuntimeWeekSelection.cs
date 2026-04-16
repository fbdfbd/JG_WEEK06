using System;

public sealed class RuntimeWeekSelection
{
    public RuntimeWeekSelection(SO_CardInfoDefinition cardDefinition, int selectedOptionIndex)
    {
        CardDefinition = cardDefinition;
        SelectedOptionIndex = selectedOptionIndex;
    }

    public SO_CardInfoDefinition CardDefinition { get; }
    public int SelectedOptionIndex { get; }

    public CardOptionData GetSelectedOption()
    {
        if (CardDefinition == null)
        {
            throw new InvalidOperationException("Card definition is missing.");
        }

        CardOptionData[] options = CardDefinition.Options;
        if (options == null || options.Length == 0)
        {
            throw new InvalidOperationException($"Card '{CardDefinition.name}' has no options.");
        }

        if (SelectedOptionIndex < 0 || SelectedOptionIndex >= options.Length)
        {
            throw new InvalidOperationException(
                $"Selected option index '{SelectedOptionIndex}' is out of range for card '{CardDefinition.name}'.");
        }

        return options[SelectedOptionIndex];
    }
}
