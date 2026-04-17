using UnityEngine;

public abstract class UI_ViewBase : MonoBehaviour
{
    protected bool isInitialized;
    public bool IsOpen { get; private set; }

    public void Init()
    {
        if (isInitialized) return;
        OnInit();
        isInitialized = true;
    }

    public virtual void Open()
    {
        Init();
        gameObject.SetActive(true);
        IsOpen = true;
        OnOpen();
    }

    public virtual void Close()
    {
        IsOpen = false;
        OnClose();
        gameObject.SetActive(false);
    }

    public virtual void Refresh()
    {
        OnRefresh();
    }

    protected abstract void OnInit();
    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }
    protected virtual void OnRefresh() { }
}