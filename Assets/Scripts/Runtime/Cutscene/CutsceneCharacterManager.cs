using System;
using UnityEngine;

public enum CutsceneCharacterType
{
    None,
    Nemo,
    Friend01,
    Friend02,
    Friend03,
    Cousin01,
    Cousin02,
    Archivist,
    Gardener,
    GuardCaptain,
    Maid,
    Secretary,
    Steward,
    StoreKeeper,
}

public class CutsceneCharacterManager : MonoBehaviour
{
    [Serializable]
    private class CharacterPrefabEntry
    {
        public CutsceneCharacterType type;
        public GameObject prefab;
    }

    [Header("Character Prefabs")]
    [SerializeField] private CharacterPrefabEntry[] _characterPrefabs = Array.Empty<CharacterPrefabEntry>();

    [Header("Spawn Points")]
    [SerializeField] private Transform _leftSpawnPoint;
    [SerializeField] private Transform _rightSpawnPoint;

    private GameObject _leftCharacterInstance;
    private GameObject _rightCharacterInstance;

    public void ShowLeft(CutsceneCharacterType characterType)
    {
        _leftCharacterInstance = ReplaceCharacter(_leftCharacterInstance, characterType, _leftSpawnPoint);
    }

    public void ShowRight(CutsceneCharacterType characterType)
    {
        _rightCharacterInstance = ReplaceCharacter(_rightCharacterInstance, characterType, _rightSpawnPoint);
    }

    public void HideAll()
    {
        HideLeft();
        HideRight();
    }

    public void HideLeft()
    {
        if (_leftCharacterInstance == null) return;

        Destroy(_leftCharacterInstance);
        _leftCharacterInstance = null;
    }

    public void HideRight()
    {
        if (_rightCharacterInstance == null) return;

        Destroy(_rightCharacterInstance);
        _rightCharacterInstance = null;
    }

    public GameObject GetLeftInstance()
    {
        return _leftCharacterInstance;
    }
    public GameObject GetRightInstance()
    {
        return _rightCharacterInstance;
    }

    private GameObject ReplaceCharacter(GameObject currentInstance, CutsceneCharacterType characterType, Transform spawnPoint)
    {
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

        if (characterType == CutsceneCharacterType.None) return null;
        if (spawnPoint == null) return null;

        GameObject prefab = FindPrefab(characterType);
        if (prefab == null) return null;

        return Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
    }

    private GameObject FindPrefab(CutsceneCharacterType characterType)
    {
        for (int i = 0; i < _characterPrefabs.Length; i++)
        {
            CharacterPrefabEntry entry = _characterPrefabs[i];
            if (entry == null) continue;
            if (entry.type != characterType) continue;

            return entry.prefab;
        }

        return null;
    }
}
