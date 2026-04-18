using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class NemoEntity : MonoBehaviour, IPointerClickHandler
{
    public static NemoEntity Instance { get; private set; }

    [Header("Animation")]
    [SerializeField] private Animator playerAnimator;

    [Header("Interaction")]
    [SerializeField] private GameObject interactPanel;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;

    [Header("Room Bounds")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Random Action Weights")]
    [SerializeField] private int idleWeight = 50;
    [SerializeField] private int balanceWeight = 30;
    [SerializeField] private int turnWeight = 20;

    private NemoAnimation _anim;
    private NemoInteract _interact;
    private NemoRoutine _routine;
    private Coroutine _dailyRoutineCoroutine;

    public bool IsInitialized => _anim != null && _interact != null && _routine != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _anim = new NemoAnimation(playerAnimator);
        _interact = new NemoInteract(interactPanel);
        _routine = new NemoRoutine(
            transform,
            _anim,
            walkSpeed,
            leftEdge,
            rightEdge,
            idleWeight,
            balanceWeight,
            turnWeight
        );
    }

    private void Start()
    {
        if (_routine == null) Debug.Log("NemoRoutine이 null임");
        if (_interact == null) Debug.Log("NemoInteract이 null임");
        if (_anim == null) Debug.Log("NemoAnimation이 null임");

        StartDailyRoutine();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _interact.HandlePointerClick(eventData);
    }

    public void StartDailyRoutine()
    {
        if (!IsInitialized)
        {
            Debug.Log("클래스 중 하나가 없음");
            return;
        }

        if (_dailyRoutineCoroutine != null)
        {
            StopCoroutine(_dailyRoutineCoroutine);
        }

        Debug.Log("기본 애니메이션 동작 시작");
        _routine.ResetRoutineState();
        _dailyRoutineCoroutine = StartCoroutine(_routine.DailyRoutine());
    }

    public void StopDailyRoutine()
    {
        if (_dailyRoutineCoroutine != null)
        {
            StopCoroutine(_dailyRoutineCoroutine);
            _dailyRoutineCoroutine = null;
        }

        _routine?.StopAction();
    }

    public void OpenInteractionPanel()
    {
        _interact.OpenPanel();
    }

    public void CloseInteractionPanel()
    {
        _interact.ClosePanel();
    }

    public void PlayIdle()
    {
        _anim.PlayIdle();
    }

    public void PlayWalk()
    {
        _anim.PlayWalk();
    }

    public void PlayBalance()
    {
        _anim.PlayBalance();
    }

    public void PlayRandomAction()
    {
        _routine.PlayRandomBehavior();
    }

    public void Turn()
    {
        _routine.ChangeMoveDirection();
    }
}
