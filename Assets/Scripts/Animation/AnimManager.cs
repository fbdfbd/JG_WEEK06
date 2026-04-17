using UnityEngine;
using UnityEngine.InputSystem;

public class AnimManager : MonoBehaviour
{
    public static AnimManager Instance;
    
    [SerializeField] private Animator playerAnim;

    private readonly int hashIdle = Animator.StringToHash("Idle");
    private readonly int hashWalk = Animator.StringToHash("Walk");
    private readonly int hashBalance = Animator.StringToHash("Balance");

    private void Start()
    {
        if (Instance != null) Destroy(gameObject);

        Instance = this;
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
