using TMPro;
using UnityEngine;
using Unity.Services.Vivox;

public class PlayerEntryUI : MonoBehaviour
{
    [SerializeField] private GameObject _audioSprite; // l'objet à activer quand il parle

    private float _checkCooldown = 0.25f; // toutes les 0.25s => 4x/sec
    private float _nextCheckTime = 0f;
    private VivoxParticipant _participant;

    public void Setup(VivoxParticipant participant)
    {
        _participant = participant;
        //_nameText.text = string.IsNullOrEmpty(participant.DisplayName) ? participant.PlayerId : participant.DisplayName;
    }

    public void Cleanup()
    {
        _participant = null;
    }

    private void Update()
    {
        if (Time.time >= _nextCheckTime && _participant != null)
        {
            _nextCheckTime = Time.time + _checkCooldown;

            bool speaking = _participant.SpeechDetected && _participant.AudioEnergy > 0.1f;
            _audioSprite.SetActive(speaking);
        }
    }
}
