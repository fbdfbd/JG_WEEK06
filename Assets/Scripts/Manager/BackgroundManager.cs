using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BackgroundType
{
    BedRoom,
    BallRoom,
    DiningRoom,
    DrawingRoom,
    Garden,
    Lake,
    Study,
}

public enum BackgroundTransitionMode
{
    None,
    Slide,
    Pop,
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager I { get; private set; }

    [Serializable]
    private class BackgroundEntry
    {
        public BackgroundType type;
        public Image targetImage;

        [NonSerialized] public RectTransform rect;
        [NonSerialized] public Vector2 initialAnchoredPosition;
    }

    [Header("Backgrounds")]
    [SerializeField] private BackgroundEntry[] backgrounds;
    [SerializeField] private BackgroundType defaultBackground;

    [Header("Transition")]
    [SerializeField] private BackgroundTransitionMode transitionMode = BackgroundTransitionMode.Slide;
    [SerializeField] private UINoBackgroundTransition noTransition;
    [SerializeField] private UIBackgroundSlideTransition slideTransition;
    [SerializeField] private UIBackgroundPopTransition popTransition;

    private readonly Dictionary<BackgroundType, BackgroundEntry> backgroundMap = new();

    private BackgroundEntry currentEntry;
    private BackgroundType currentBackground;
    private bool isTransitioning;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;

        BuildMap();
        Initialize(defaultBackground);
    }

    public void ShowBackground(BackgroundType type)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("지금 배경 전환 중입니다.");
            return;
        }

        if (!TryGetEntry(type, out BackgroundEntry nextEntry))
            return;

        if (currentEntry != null && currentEntry.type == type)
            return;

        UIBackgroundTransitionBase transition = GetSelectedTransition();

        if (currentEntry == null || transition == null)
        {
            SwitchImmediately(nextEntry);
            return;
        }

        isTransitioning = true;
        BackgroundEntry previousEntry = currentEntry;

        transition.Play(
            previousEntry.rect,
            previousEntry.initialAnchoredPosition,
            nextEntry.rect,
            nextEntry.initialAnchoredPosition,
            () =>
            {
                currentEntry = nextEntry;
                currentBackground = nextEntry.type;
                isTransitioning = false;
            });
    }

    public void HideBackground(BackgroundType type)
    {
        if (currentBackground != type)
            return;

        ShowBackground(defaultBackground);
    }

    public void SetTransitionMode(BackgroundTransitionMode mode)
    {
        transitionMode = mode;
    }

    private UIBackgroundTransitionBase GetSelectedTransition()
    {
        switch (transitionMode)
        {
            case BackgroundTransitionMode.None:
                return noTransition;

            case BackgroundTransitionMode.Pop:
                return popTransition;

            case BackgroundTransitionMode.Slide:
            default:
                return slideTransition;
        }
    }

    private void BuildMap()
    {
        backgroundMap.Clear();

        if (backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogWarning("backgrounds 배열이 비어 있습니다.");
            return;
        }

        for (int i = 0; i < backgrounds.Length; i++)
        {
            BackgroundEntry entry = backgrounds[i];

            if (entry == null)
            {
                Debug.LogWarning($"backgrounds[{i}]가 null입니다.");
                continue;
            }

            if (entry.targetImage == null)
            {
                Debug.LogWarning($"backgrounds[{i}] ({entry.type}) 의 Image가 비어 있습니다.");
                continue;
            }

            if (backgroundMap.ContainsKey(entry.type))
            {
                Debug.LogWarning($"중복된 배경 타입입니다: {entry.type}");
                continue;
            }

            RectTransform rect = entry.targetImage.rectTransform;

            if (rect == null)
            {
                Debug.LogWarning($"backgrounds[{i}] ({entry.type}) 의 RectTransform을 찾을 수 없습니다.");
                continue;
            }

            entry.rect = rect;
            entry.initialAnchoredPosition = rect.anchoredPosition;

            backgroundMap.Add(entry.type, entry);
        }
    }

    private void Initialize(BackgroundType startType)
    {
        if (!TryGetEntry(startType, out BackgroundEntry startEntry))
            return;

        foreach (BackgroundEntry entry in backgroundMap.Values)
        {
            if (entry.rect == null)
                continue;

            entry.rect.anchoredPosition = entry.initialAnchoredPosition;
            entry.rect.localScale = Vector3.one;
            entry.targetImage.gameObject.SetActive(entry.type == startType);
        }

        startEntry.rect.SetAsLastSibling();

        currentEntry = startEntry;
        currentBackground = startType;
        isTransitioning = false;
    }

    private void SwitchImmediately(BackgroundEntry nextEntry)
    {
        foreach (BackgroundEntry entry in backgroundMap.Values)
        {
            if (entry.rect == null)
                continue;

            bool shouldShow = entry.type == nextEntry.type;

            entry.rect.anchoredPosition = entry.initialAnchoredPosition;
            entry.rect.localScale = Vector3.one;
            entry.targetImage.gameObject.SetActive(shouldShow);
        }

        nextEntry.rect.SetAsLastSibling();

        currentEntry = nextEntry;
        currentBackground = nextEntry.type;
        isTransitioning = false;
    }

    private bool TryGetEntry(BackgroundType type, out BackgroundEntry entry)
    {
        if (!backgroundMap.TryGetValue(type, out entry))
        {
            Debug.LogWarning($"등록되지 않은 배경 타입입니다: {type}");
            return false;
        }

        if (entry == null || entry.targetImage == null || entry.rect == null)
        {
            Debug.LogWarning($"배경 참조가 비어 있습니다: {type}");
            return false;
        }

        return true;
    }
}