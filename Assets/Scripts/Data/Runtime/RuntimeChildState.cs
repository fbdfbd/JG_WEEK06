using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeChildState
{
    public const int MinStatValue = 0;
    public const int MaxStatValue = 5;
    public const int DefaultStatValue = 2;

    private readonly Dictionary<EChildStatusType, int> _stats = new();
    private readonly HashSet<SO_FlagDefinition> _flags = new();
    private readonly List<string> _reactionLogs = new();

    public RuntimeChildState()
    {
        InitializeDefaultStats();
    }

    public IReadOnlyCollection<SO_FlagDefinition> Flags => _flags;
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

    public bool HasFlag(SO_FlagDefinition flagDefinition)
    {
        return flagDefinition != null && _flags.Contains(flagDefinition);
    }

    public bool HasFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
        {
            return false;
        }

        return _flags.Any(flagDefinition => flagDefinition != null && flagDefinition.Id == flagId);
    }

    public void SetFlag(SO_FlagDefinition flagDefinition)
    {
        if (flagDefinition == null)
        {
            return;
        }

        _flags.Add(flagDefinition);
    }

    public void RemoveFlag(SO_FlagDefinition flagDefinition)
    {
        if (flagDefinition == null)
        {
            return;
        }

        _flags.Remove(flagDefinition);
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

[CreateAssetMenu(
    fileName = "FlagDefinition_",
    menuName = "Scriptable Objects/Runtime/FlagDefinition")]
public class SO_FlagDefinition : ScriptableObject
{
    [SerializeField] private string _id = string.Empty;
    [SerializeField] private string _displayName = string.Empty;
    [SerializeField, TextArea(2, 4)] private string _description = string.Empty;

    public string Id => _id;
    public string DisplayName => _displayName;
    public string Description => _description;
}
