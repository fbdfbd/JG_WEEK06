using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NephewController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3;
    private Vector3 moveDirection;

    [Header("방 크기")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("랜덤 애니메이션 가중치")]
    [SerializeField] private int idleWeight = 50;
    [SerializeField] private int balanceWeight = 30;
    [SerializeField] private int turnWeight = 20;

    private bool isDoingAction = false;

    private Coroutine dailyRoutineCoroutine;

    private (Func<bool> behavior, int weight)[] weightedBehaviors;

    private void Start()
    {
        InitBehaviors();

        StartDailyRoutine();
    }

    // 랜덤 애니메이션 가중치 초기화하는 함수
    private void InitBehaviors()
    {
        weightedBehaviors = new (Func<bool>, int)[]
        {
            (() => { AnimManager.Instance.PlayIdle(); return false; }, idleWeight),
            
            (() => { AnimManager.Instance.PlayBalance(); return false; }, balanceWeight),
            
            (() => { ChangeMoveDirection(); return true; }, turnWeight)
        };
    }

    // 기본 애니메이션 움직임 
    public void StartDailyRoutine()
    {
        moveDirection = Vector3.right;

        isDoingAction = false;

        if (dailyRoutineCoroutine != null) StopCoroutine(dailyRoutineCoroutine);
        dailyRoutineCoroutine = StartCoroutine(DailyRoutine());
    }

    IEnumerator DailyRoutine()
    {
        while (!isDoingAction)
        { 
            // 기본 걷기
            AnimManager.Instance.PlayWalk();
            float walkTime = Random.Range(2f, 5f);
            float timer = 0f;

            while (timer < walkTime)
            {
                if (isDoingAction) yield break; 

                Move(); 
                timer += Time.deltaTime;

                if (transform.position.x >= rightEdge.position.x || transform.position.x <= leftEdge.position.x)
                {
                    ChangeMoveDirection();
                }

                yield return null; 
            }

            // 랜덤 행동 뽑기
            bool skipWait = RandomBehavior();

            if (skipWait) 
            {
                AnimManager.Instance.PlayWalk(); 
                continue;
            }

            float actionTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(actionTime);

            AnimManager.Instance.PlayWalk();
        }
    }

    private void ChangeMoveDirection()
    {
        moveDirection = (moveDirection == Vector3.left) ? Vector3.right : Vector3.left;

        Vector3 currentScale = transform.localScale;

        currentScale.x = (moveDirection == Vector3.left) ? -Mathf.Abs(currentScale.x) : Mathf.Abs(currentScale.x);

        transform.localScale = currentScale;
    }

    private void Move()
    {
        transform.Translate(moveDirection * walkSpeed * Time.deltaTime);
    }

    private bool RandomBehavior()
    {
        int totalWeight = 0;
        foreach (var item in weightedBehaviors) totalWeight += item.weight;

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;
        bool skipWait = false;

        foreach (var item in weightedBehaviors)
        {
            currentWeight += item.weight;
            if (randomValue < currentWeight)
            {
                skipWait = item.behavior();
                break;
            }
        }

        return skipWait;
    }
}
