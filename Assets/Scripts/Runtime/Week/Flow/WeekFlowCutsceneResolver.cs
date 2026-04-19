using System;

public sealed class WeekFlowCutsceneResolver
{
    public bool TryResolveCutsceneId(
        WeekFlowCutsceneRequest request,
        SO_WeekFlowCutsceneCatalog catalog,
        out string cutsceneId)
    {
        cutsceneId = string.Empty;
        if (catalog == null || catalog.Entries == null || catalog.Entries.Length == 0)
        {
            return false;
        }

        int bestScore = int.MinValue;

        for (int index = 0; index < catalog.Entries.Length; index++)
        {
            WeekFlowCutsceneEntryData entry = catalog.Entries[index];
            if (!IsMatch(entry, request, out int score))
            {
                continue;
            }

            if (score <= bestScore)
            {
                continue;
            }

            bestScore = score;
            cutsceneId = entry.CutsceneId;
        }

        return !string.IsNullOrWhiteSpace(cutsceneId);
    }

    private static bool IsMatch(WeekFlowCutsceneEntryData entry, WeekFlowCutsceneRequest request, out int score)
    {
        score = 0;

        if (entry == null || entry.Moment != request.Moment || string.IsNullOrWhiteSpace(entry.CutsceneId))
        {
            return false;
        }

        if (entry.UseScreenType)
        {
            if (entry.ScreenType != request.ScreenType)
            {
                return false;
            }

            score++;
        }

        if (!IsStringFieldMatch(entry.WeekId, request.WeekId, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.EventId, request.EventId, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.StepName, request.StepName, ref score))
        {
            return false;
        }

        if (!IsStringFieldMatch(entry.ChoiceLabel, request.ChoiceLabel, ref score))
        {
            return false;
        }

        if (entry.DialogueIndex >= 0)
        {
            if (entry.DialogueIndex != request.DialogueIndex)
            {
                return false;
            }

            score++;
        }

        return true;
    }

    private static bool IsStringFieldMatch(string expected, string actual, ref int score)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return true;
        }

        if (!string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        score++;
        return true;
    }
}
