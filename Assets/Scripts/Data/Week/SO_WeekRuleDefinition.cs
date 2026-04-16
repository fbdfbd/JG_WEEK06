using UnityEngine;

public abstract class SO_WeekRuleDefinition : ScriptableObject
{
    public virtual void OnWeekStart(RuntimeWeekContext context) { }

    public virtual void BeforeResolveCard(
        RuntimeWeekContext context,
        SO_CardInfoDefinition cardDefinition,
        CardOptionData selectedOption)
    { }

    public virtual void AfterResolveCard(
        RuntimeWeekContext context,
        SO_CardInfoDefinition cardDefinition,
        CardOptionData selectedOption)
    { }

    public virtual void OnWeekEnd(RuntimeWeekContext context) { }
}
