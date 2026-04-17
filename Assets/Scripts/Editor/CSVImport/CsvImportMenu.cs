using System;
using UnityEditor;
using UnityEngine;

public static class CsvImportMenu
{
    [MenuItem("Tools/CSV Import/Open Window")]
    public static void OpenWindow()
    {
        CsvImportWindow.Open();
    }

    [MenuItem("Tools/CSV Import/Import All")]
    public static void ImportAll()
    {
        RunImport(validateOnly: false);
    }

    [MenuItem("Tools/CSV Import/Validate CSV")]
    public static void ValidateOnly()
    {
        RunImport(validateOnly: true);
    }

    public static void RunImport(bool validateOnly)
    {
        try
        {
            CsvImportSettings settings = CsvImportWindow.LoadSettings();
            settings.ValidateOnly = validateOnly;
            CsvImportReport report = CsvImportPipeline.Run(settings);
            Debug.Log(validateOnly
                ? $"[CSV Import] Validation passed.\n{report.BuildSummary()}"
                : $"[CSV Import] Import complete.\n{report.BuildSummary()}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"[CSV Import] Failed.\n{exception}");
            EditorUtility.DisplayDialog("CSV Import", exception.Message, "OK");
        }
    }
}
