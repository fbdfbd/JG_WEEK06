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

public enum CutsceneParticleType
{
    None,
    Happy,
    Heart,
    Line,
    Melancholy,
    Sad,
}

public enum CutsceneCharacterPos
{
    None,
    Left,
    Center,
    Right,
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

    [Serializable]
    private class ParticlePrefabEntry
    {
        public CutsceneParticleType type;
        public ParticleSystem target;
    }

    [Header("Character Prefabs")]
    [SerializeField] private CharacterPrefabEntry[] _characterPrefabs = Array.Empty<CharacterPrefabEntry>();
    [SerializeField] private ParticlePrefabEntry[] _particlePrefabs = Array.Empty<ParticlePrefabEntry>();

    [Header("Spawn Points")]
    [SerializeField] private Transform _leftSpawnPoint;
    [SerializeField] private Transform _rightSpawnPoint;
    [SerializeField] private Transform _centerSpawnPoint;

    private GameObject _leftCharacterInstance;
    private GameObject _rightCharacterInstance;
    private GameObject _centerCharacterInstance;
    private ParticleSystem _readyParticle;

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

    public void ShowCenter(CutsceneCharacterType characterType)
    {
        GameObject character = FindPrefab(characterType);
        if (character == null)
        {
            _centerCharacterInstance = null;
            return;
        }

        ShowCharacter(character, _centerSpawnPoint, false);
        _centerCharacterInstance = character;
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
        HideCenter();
    }

    public void HideLeft()
    {
        if (_leftCharacterInstance == null) return;

        _leftCharacterInstance.SetActive(false);
        _leftCharacterInstance = null;
    }

    public void HideCenter()
    {
        if (_centerCharacterInstance == null) return;

        _centerCharacterInstance.SetActive(false);
        _centerCharacterInstance = null;
    }

    public void HideRight()
    {
        if (_rightCharacterInstance == null) return;

        _rightCharacterInstance.SetActive(false);
        _rightCharacterInstance = null;
    }

    public void PlayParticleOnLeft(CutsceneParticleType particleType)
    {
        PlayParticle(particleType, _leftCharacterInstance);
    }
    public void PlayParticleOnCenter(CutsceneParticleType particleType)
    {
        PlayParticle(particleType, _centerCharacterInstance);
    }
    public void PlayParticleOnRight(CutsceneParticleType particleType)
    {
        PlayParticle(particleType, _rightCharacterInstance);
    }

    public void PlayParticle(CutsceneParticleType particleType, GameObject character)
    {
        if (particleType == CutsceneParticleType.None) return;
        if (character == null) return;

        ParticleSystem particle = FindParticlePrefab(particleType);
        if(particle == null) return;

        particle.gameObject.transform.position = character.transform.position;

        particle.Play();
    }

    public GameObject GetLeftInstance()
    {
        return _leftCharacterInstance;
    }

    public GameObject GetCenterInstance()
    {
        return _centerCharacterInstance;
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

    private ParticleSystem FindParticlePrefab(CutsceneParticleType particleType)
    {
        if (particleType == CutsceneParticleType.None) return null;

        for (int i = 0; i < _particlePrefabs.Length; i++)
        {
            ParticlePrefabEntry entry = _particlePrefabs[i];
            if (entry == null) continue;
            if (entry.type != particleType) continue;

            return entry.target;
        }

        return null;
    }
}
