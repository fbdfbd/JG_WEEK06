using System;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class NemoAnimation
{
    private readonly Animator _playerAnim;

    private static readonly int HashIdle = Animator.StringToHash("Idle");
    private static readonly int HashWalk = Animator.StringToHash("Walk");
    private static readonly int HashBalance = Animator.StringToHash("Balance");

    public NemoAnimation(Animator playerAnim)
    {
        _playerAnim = playerAnim;
    }

    public void PlayRandomAction()
    {
        Action[] randomActions =
        {
            PlayIdle,
            PlayBalance
        };

        int randomIndex = Random.Range(0, randomActions.Length);
        randomActions[randomIndex].Invoke();
    }

    public void PlayIdle()
    {
        if (_playerAnim != null)
        {
            _playerAnim.SetTrigger(HashIdle);
        }
    }

    public void PlayWalk()
    {
        if (_playerAnim != null)
        {
            _playerAnim.SetTrigger(HashWalk);
        }
    }

    public void PlayBalance()
    {
        if (_playerAnim != null)
        {
            _playerAnim.SetTrigger(HashBalance);
        }
    }
}
