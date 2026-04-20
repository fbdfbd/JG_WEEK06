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
    public bool HasPendingEventReward =>
        !HasAppliedEventReward &&
        SelectedOption?.Interactions != null &&
        SelectedOption.Interactions.Length > 0;

    public bool HasAppliedEventReward { get; private set; }

    public void ApplyInteractionsTo(RuntimeChildState childState)
    {
        if (childState == null || SelectedOption?.Interactions == null)
        {
            return;
        }

        GameplayInteractionExecutor.ApplyAll(SelectedOption.Interactions, childState);
    }

    public bool TryApplyPendingEventReward(RuntimeChildState childState)
    {
        if (!HasPendingEventReward || childState == null)
        {
            return false;
        }

        GameplayInteractionExecutor.ApplyAll(SelectedOption.Interactions, childState);
        HasAppliedEventReward = true;
        return true;
    }
}
