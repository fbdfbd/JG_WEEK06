using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimManager : MonoBehaviour
{
    public static AnimManager Instance;
    
    [SerializeField] private Animator playerAnim;

    private readonly int hashIdle = Animator.StringToHash("Idle");
    private readonly int hashWalk = Animator.StringToHash("Walk");
    private readonly int hashBalance = Animator.StringToHash("Balance");

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);

        Instance = this;
    }

    public void PlayRandomAction()
    {
        Action[] randomActions = new Action[]
        {
            PlayIdle,
            PlayBalance,
        };

        int randomIndex = Random.Range(0, randomActions.Length);
        randomActions[randomIndex]();
    }

    public void PlayIdle()
    {
        if (playerAnim != null) playerAnim.SetTrigger(hashIdle);
    }
    public void PlayWalk()
    {
        if (playerAnim != null) playerAnim.SetTrigger(hashWalk);
    }
    public void PlayBalance()
    {
        if (playerAnim != null) playerAnim.SetTrigger(hashBalance);
    }
}
