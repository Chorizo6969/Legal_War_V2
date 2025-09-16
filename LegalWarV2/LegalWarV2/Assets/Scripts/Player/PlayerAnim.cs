using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnim : NetworkBehaviour
{
    [SerializeField] private Animator _animator;

    public void TransitionTo(AnimEnum animEnum, bool result)
    {
        if (!IsOwner)
            return;
        _animator.SetBool(animEnum.ToString(), result);
    }
}
