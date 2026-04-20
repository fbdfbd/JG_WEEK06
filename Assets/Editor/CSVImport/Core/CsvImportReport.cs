using System.Collections.Generic;
using System.Linq;

public sealed class CsvImportReport
{
    private readonly List<string> _infos = new();
    private readonly List<string> _warnings = new();

    public int CreatedCount { get; private set; }
    public int UpdatedCount { get; private set; }
    public int ValidationErrorCount { get; set; }

    public IReadOnlyList<string> Infos => _infos;
    public IReadOnlyList<string> Warnings => _warnings;

    public void RecordCreated(string message)
    {
        CreatedCount++;
        _infos.Add(message);
    }

    public void RecordUpdated(string message)
    {
        UpdatedCount++;
        _infos.Add(message);
    }

    public void RecordWarning(string message)
    {
        _warnings.Add(message);
    }

    public string BuildSummary()
    {
        IEnumerable<string> warningSection = _warnings.Count == 0
            ? new[] { "Warnings: 0" }
            : new[] { $"Warnings: {_warnings.Count}" }.Concat(_warnings.Select(warning => $"- {warning}"));

        return string.Join(
            "\n",
            new[]
            {
                $"Created: {CreatedCount}",
                $"Updated: {UpdatedCount}",
                $"Validation Errors: {ValidationErrorCount}",
            }.Concat(warningSection));
    }
}
