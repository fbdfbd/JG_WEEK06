using System;
using UnityEngine;

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

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager I { get; private set; }

    [Serializable]
    private class BackgroundEntry
    {
        public BackgroundType type;
        public GameObject target;
    }

    [SerializeField] private BackgroundEntry[] backgrounds;
    [SerializeField] private BackgroundType defaultBackground;

    private BackgroundType currentBackground;

    private void Awake()
    {
        I = this;
        Show(defaultBackground);
    }

    public void ShowBackground(BackgroundType type)
    {
        if (I == null)
        {
            Debug.LogWarning("BackgroundManager가 없습니다.");
            return;
        }

        I.Show(type);
    }

    public void HideBackground(BackgroundType type)
    {
        if (I == null)
        {
            Debug.LogWarning("BackgroundManager가 없습니다.");
            return;
        }

        I.Hide(type);
    }

    private void Show(BackgroundType type)
    {
        if (backgrounds == null || backgrounds.Length == 0)
            return;

        bool found = false;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] == null || backgrounds[i].target == null)
                continue;

            bool isTarget = backgrounds[i].type == type;
            backgrounds[i].target.SetActive(isTarget);

            if (isTarget)
                found = true;
        }

        if (!found)
        {
            Debug.LogWarning($"등록되지 않은 배경 타입입니다: {type}");
            return;
        }

        currentBackground = type;
    }

    private void Hide(BackgroundType type)
    {
        if (currentBackground != type)
            return;

        Show(defaultBackground);
    }
}