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
    public static CutsceneCharacterManager I { get; private set; }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

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
        GameObject character = FindPrefab(characterType);
        if (character == null)
        {
            _leftCharacterInstance = null;
            return;
        }

        ShowCharacter(character, _leftSpawnPoint, false);
        _leftCharacterInstance = character;
    }

    public void ShowRight(CutsceneCharacterType characterType)
    {
        GameObject character = FindPrefab(characterType);
        if (character == null)
        {
            _rightCharacterInstance = null;
            return;
        }

        ShowCharacter(character, _rightSpawnPoint, true);
        _rightCharacterInstance = character;
    }

    public void HideAll()
    {
        HideLeft();
        HideRight();
    }

    public void HideLeft()
    {
        if (_leftCharacterInstance == null) return;

        _leftCharacterInstance.SetActive(false);
        _leftCharacterInstance = null;
    }

    public void HideRight()
    {
        if (_rightCharacterInstance == null) return;

        _rightCharacterInstance.SetActive(false);
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

    private void ShowCharacter(GameObject character, Transform spawnPoint, bool flipX)
    {
        if (character == null || spawnPoint == null)
        {
            return;
        }

        character.transform.position = spawnPoint.position;
        character.transform.rotation = spawnPoint.rotation;

        SetFlip(character.transform, flipX);

        character.SetActive(true);
    }

    private void SetFlip(Transform target, bool flipX)
    {
        Vector3 scale = target.localScale;
        float absX = Mathf.Abs(scale.x);
        scale.x = flipX ? -absX : absX;
        target.localScale = scale;
    }

    private GameObject FindPrefab(CutsceneCharacterType characterType)
    {
        if (characterType == CutsceneCharacterType.None) return null;

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
