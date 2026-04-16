using System.Collections.Generic;

public static class GameplayInteractionExecutor
{
    public static void ApplyAll(
        IReadOnlyList<SO_CardInteractionDefinition> interactions,
        RuntimeChildState childState)
    {
        if (interactions == null || childState == null)
        {
            return;
        }

        foreach (SO_CardInteractionDefinition interaction in interactions)
        {
            interaction?.Apply(childState);
        }
    }
}
