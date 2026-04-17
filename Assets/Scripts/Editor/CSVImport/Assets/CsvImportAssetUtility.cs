using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CsvImportAssetUtility
{
    private static readonly HashSet<string> EnsuredFolders = new(StringComparer.OrdinalIgnoreCase);

    public static void BeginImportSession()
    {
        EnsuredFolders.Clear();
    }

    public static void EndImportSession()
    {
        EnsuredFolders.Clear();
    }

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
        string normalizedPath = NormalizeAssetPath(folderPath);
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return;
        }

        if (EnsuredFolders.Contains(normalizedPath))
        {
            return;
        }

        if (AssetDatabase.IsValidFolder(normalizedPath))
        {
            EnsuredFolders.Add(normalizedPath);
            return;
        }

        string parentFolder = NormalizeAssetPath(Path.GetDirectoryName(normalizedPath));
        string folderName = Path.GetFileName(normalizedPath);

        EnsureFolder(parentFolder);
        if (!AssetDatabase.IsValidFolder(normalizedPath))
        {
            AssetDatabase.CreateFolder(parentFolder, folderName);
        }

        EnsuredFolders.Add(normalizedPath);
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

    private static string NormalizeAssetPath(string assetPath)
    {
        return assetPath?.Replace('\\', '/');
    }
}
