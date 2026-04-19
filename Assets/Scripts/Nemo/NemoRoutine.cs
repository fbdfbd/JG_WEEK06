using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NemoRoutine
{
    private readonly Transform _transform;
    private readonly NemoAnimation _anim;
    private readonly float _walkSpeed;
    private readonly Transform _leftEdge;
    private readonly Transform _rightEdge;
    private readonly Func<bool> _canRunRoutine;

    private readonly (Func<bool> behavior, int weight)[] _weightedBehaviors;

    private Vector3 _moveDirection = Vector3.right;

    public NemoRoutine(
        Transform transform,
        NemoAnimation anim,
        float walkSpeed,
        Transform leftEdge,
        Transform rightEdge,
        int idleWeight,
        int balanceWeight,
        int turnWeight,
        Func<bool> canRunRoutine)
    {
        _transform = transform;
        _anim = anim;
        _walkSpeed = walkSpeed;
        _leftEdge = leftEdge;
        _rightEdge = rightEdge;
        _canRunRoutine = canRunRoutine;

        _weightedBehaviors = new (Func<bool>, int)[]
        {
            (() =>
            {
                _anim.PlayIdle();
                return false;
            }, idleWeight),

            (() =>
            {
                _anim.PlayBalance();
                return false;
            }, balanceWeight),

            (() =>
            {
                ChangeMoveDirection();
                return true;
            }, turnWeight)
        };
    }

    public IEnumerator DailyRoutine()
    {
        while (_canRunRoutine())
        {
            _anim.PlayWalk();

            float walkTime = Random.Range(2f, 5f);
            float timer = 0f;

            while (timer < walkTime)
            {
                if (!_canRunRoutine())
                {
                    yield break;
                }

                Move();
                timer += Time.deltaTime;

                if (IsOutOfBounds())
                {
                    ChangeMoveDirection();
                }

                yield return null;
            }

            if (!_canRunRoutine())
            {
                yield break;
            }

            bool skipWait = RandomBehavior();

            if (skipWait)
            {
                _anim.PlayWalk();
                continue;
            }

            float actionTime = Random.Range(2f, 5f);
            float actionTimer = 0f;

            while (actionTimer < actionTime)
            {
                if (!_canRunRoutine())
                {
                    yield break;
                }

                actionTimer += Time.deltaTime;
                yield return null;
            }

            _anim.PlayWalk();
        }
    }

    public void PlayRandomBehavior()
    {
        RandomBehavior();
    }

    public void ChangeMoveDirection()
    {
        _moveDirection = _moveDirection == Vector3.left ? Vector3.right : Vector3.left;

        Vector3 currentScale = _transform.localScale;
        currentScale.x = _moveDirection == Vector3.left
            ? -Mathf.Abs(currentScale.x)
            : Mathf.Abs(currentScale.x);

        _transform.localScale = currentScale;
    }

    private void Move()
    {
        _transform.Translate(_moveDirection * _walkSpeed * Time.deltaTime);
    }

    private bool IsOutOfBounds()
    {
        if (_leftEdge == null || _rightEdge == null)
        {
            return false;
        }

        bool hitRightEdge = _transform.position.x >= _rightEdge.position.x && _moveDirection == Vector3.right;
        bool hitLeftEdge = _transform.position.x <= _leftEdge.position.x && _moveDirection == Vector3.left;

        return hitRightEdge || hitLeftEdge;
    }

    private bool RandomBehavior()
    {
        int totalWeight = 0;
        foreach (var item in _weightedBehaviors)
        {
            totalWeight += item.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var item in _weightedBehaviors)
        {
            currentWeight += item.weight;
            if (randomValue < currentWeight)
            {
                return item.behavior.Invoke();
            }
        }

        return false;
    }
}
