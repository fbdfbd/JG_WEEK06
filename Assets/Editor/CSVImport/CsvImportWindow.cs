using UnityEditor;
using UnityEngine;

public sealed class CsvImportWindow : EditorWindow
{
    private const string CsvRootKey = "CSVImport.CsvRoot";
    private const string OutputRootKey = "CSVImport.OutputRoot";

    private CsvImportSettings _settings;

    public static void Open()
    {
        CsvImportWindow window = GetWindow<CsvImportWindow>("CSV Import");
        window.minSize = new Vector2(520f, 180f);
        window.Show();
    }

    public static CsvImportSettings LoadSettings()
    {
        return new CsvImportSettings
        {
            CsvRootPath = EditorPrefs.GetString(CsvRootKey, CsvImportSettings.DefaultCsvRootPath),
            OutputRootPath = EditorPrefs.GetString(OutputRootKey, CsvImportSettings.DefaultOutputRootPath),
        };
    }

    private void OnEnable()
    {
        _settings = LoadSettings();
    }

    private void OnGUI()
    {
        if (_settings == null)
        {
            _settings = LoadSettings();
        }

        EditorGUILayout.LabelField("CSV > SO Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space(6f);

        _settings.CsvRootPath = EditorGUILayout.TextField("CSV Root", _settings.CsvRootPath);
        _settings.OutputRootPath = EditorGUILayout.TextField("Output Root", _settings.OutputRootPath);

        EditorGUILayout.Space(10f);
        EditorGUILayout.HelpBox(
            "weeks.csv 는 주차 루트와 PreTurn 컨테이너를 만들고,\n" +
            "events/event_steps/event_*_dialogue_lines 가 실제 낮/밤 이벤트 내용을 채웁니다.",
            MessageType.Info);

        EditorGUILayout.Space(10f);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Validate CSV", GUILayout.Height(30f)))
            {
                SaveSettings();
                CsvImportMenu.RunImport(validateOnly: true);
            }

            if (GUILayout.Button("Import All", GUILayout.Height(30f)))
            {
                SaveSettings();
                CsvImportMenu.RunImport(validateOnly: false);
            }
        }
    }

    private void SaveSettings()
    {
        EditorPrefs.SetString(CsvRootKey, _settings.CsvRootPath);
        EditorPrefs.SetString(OutputRootKey, _settings.OutputRootPath);
    }
}
