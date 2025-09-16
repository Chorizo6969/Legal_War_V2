using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void TransitionTo(AnimEnum animEnum, bool result)
    {
        _animator.SetBool(animEnum.ToString(), result);
    }
}
