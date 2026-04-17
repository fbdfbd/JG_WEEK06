using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CsvImportAssetUtility
{
    public static T LoadOrCreateAsset<T>(string assetPath, string assetName, CsvImportReport report)
        where T : ScriptableObject
    {
        T existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (existing != null)
        {
            existing.name = assetName;
            EditorUtility.SetDirty(existing);
            report.RecordUpdated($"Updated {typeof(T).Name}: {assetPath}");
            return existing;
        }

        EnsureFolder(Path.GetDirectoryName(assetPath)?.Replace('\\', '/'));
        T created = ScriptableObject.CreateInstance<T>();
        created.name = assetName;
        AssetDatabase.CreateAsset(created, assetPath);
        report.RecordCreated($"Created {typeof(T).Name}: {assetPath}");
        return created;
    }

    public static void EnsureFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string normalizedPath = folderPath.Replace('\\', '/');
        string parentFolder = Path.GetDirectoryName(normalizedPath)?.Replace('\\', '/');
        string folderName = Path.GetFileName(normalizedPath);

        EnsureFolder(parentFolder);
        if (!AssetDatabase.IsValidFolder(normalizedPath))
        {
            AssetDatabase.CreateFolder(parentFolder, folderName);
        }
    }

    public static string CombineAssetPath(params string[] parts)
    {
        return string.Join("/", parts).Replace("\\", "/");
    }

    public static void SetField(object target, string fieldName, object value)
    {
        Type currentType = target.GetType();
        while (currentType != null)
        {
            FieldInfo field = currentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            currentType = currentType.BaseType;
        }

        throw new InvalidOperationException($"Field not found: {target.GetType().Name}.{fieldName}");
    }

    public static void MarkDirty(UnityEngine.Object asset)
    {
        if (asset != null)
        {
            EditorUtility.SetDirty(asset);
        }
    }
}
