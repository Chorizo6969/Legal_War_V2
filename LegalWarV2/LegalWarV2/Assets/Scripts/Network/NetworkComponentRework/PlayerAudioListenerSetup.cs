using Unity.Netcode;
using UnityEngine;

public class PlayerAudioListenerSetup : NetworkBehaviour
{
    [SerializeField] private AudioListener _audioListener;
    [SerializeField] private Camera _playerCamera;

    private void Start()
    {
        if (IsOwner)
        {
            _playerCamera.gameObject.SetActive(true);

            if (_audioListener != null)
                _audioListener.enabled = true;
        }
        else
        {
            _playerCamera.gameObject.SetActive(false);

            if (_audioListener != null)
                _audioListener.enabled = false;
        }

    }
}
