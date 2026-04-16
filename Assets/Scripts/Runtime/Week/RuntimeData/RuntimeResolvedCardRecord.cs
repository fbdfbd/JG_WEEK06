public sealed class RuntimeResolvedCardRecord
{
    public RuntimeResolvedCardRecord(
        SO_CardInfoDefinition cardDefinition,
        int selectedOptionIndex,
        CardOptionData selectedOption)
    {
        CardDefinition = cardDefinition;
        SelectedOptionIndex = selectedOptionIndex;
        SelectedOption = selectedOption;
    }

    public SO_CardInfoDefinition CardDefinition { get; }
    public int SelectedOptionIndex { get; }
    public CardOptionData SelectedOption { get; }
}
