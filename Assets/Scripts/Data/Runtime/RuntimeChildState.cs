using System;
using System.Collections.Generic;

public class RuntimeChildState
{
    public const int MinStatValue = 0;
    public const int MaxStatValue = 5;
    public const int DefaultStatValue = 2;

    private readonly Dictionary<EChildStatusType, int> _stats = new();
    private readonly HashSet<EChildFlagType> _flags = new();
    private readonly List<string> _reactionLogs = new();

    public RuntimeChildState()
    {
        InitializeDefaultStats();
    }

    public IReadOnlyCollection<EChildFlagType> Flags => _flags;
    public IReadOnlyList<string> ReactionLogs => _reactionLogs;

    public int GetStat(EChildStatusType statType)
    {
        if (_stats.TryGetValue(statType, out int value))
        {
            return value;
        }

        return DefaultStatValue;
    }

    public void SetStat(EChildStatusType statType, int value)
    {
        _stats[statType] = ClampStat(value);
    }

    public void AddStat(EChildStatusType statType, int amount)
    {
        SetStat(statType, GetStat(statType) + amount);
    }

    public bool HasFlag(EChildFlagType flagType)
    {
        return _flags.Contains(flagType);
    }

    public void SetFlag(EChildFlagType flagType)
    {
        if (flagType == EChildFlagType.None)
        {
            return;
        }

        _flags.Add(flagType);
    }

    public void RemoveFlag(EChildFlagType flagType)
    {
        _flags.Remove(flagType);
    }

    public void AddReactionLog(string reactionText)
    {
        if (string.IsNullOrWhiteSpace(reactionText))
        {
            return;
        }

        _reactionLogs.Add(reactionText);
    }

    public void ClearReactionLogs()
    {
        _reactionLogs.Clear();
    }

    private void InitializeDefaultStats()
    {
        foreach (EChildStatusType statType in Enum.GetValues(typeof(EChildStatusType)))
        {
            _stats[statType] = DefaultStatValue;
        }
    }

    private static int ClampStat(int value)
    {
        if (value < MinStatValue)
        {
            return MinStatValue;
        }

        if (value > MaxStatValue)
        {
            return MaxStatValue;
        }

        return value;
    }
}

public enum EChildStatusType
{
    Trust,
    Curiosity,
    Anxiety,
    Obedience
}

public enum EChildFlagType
{
    None = 0,
    LetterSuspected,
    VisitorRemembered,
    ExternalInterest,
    HiddenInfoDetected
}
