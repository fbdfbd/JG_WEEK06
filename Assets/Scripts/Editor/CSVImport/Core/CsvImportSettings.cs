public sealed class CsvImportSettings
{
    public const string DefaultCsvRootPath = "Assets/Data/CSV";
    public const string DefaultOutputRootPath = "Assets/Data/Generated/CSVImport";

    public string CsvRootPath { get; set; } = DefaultCsvRootPath;
    public string OutputRootPath { get; set; } = DefaultOutputRootPath;
    public bool ValidateOnly { get; set; }
}
