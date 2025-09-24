using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;

public class VivoxRosterManager : MonoBehaviour
{
    [SerializeField] private List<PlayerEntryUI> _playerEntryUIList = new();
    private int _numberPlayerInGame = 0;

    private readonly Dictionary<string, PlayerEntryUI> _entries = new();

    private void OnEnable()
    {
        VivoxService.Instance.ParticipantAddedToChannel += OnParticipantAdded;
        VivoxService.Instance.ParticipantRemovedFromChannel += OnParticipantRemoved;
    }

    private void OnDisable()
    {
        if (VivoxService.Instance != null)
        {
            VivoxService.Instance.ParticipantAddedToChannel -= OnParticipantAdded;
            VivoxService.Instance.ParticipantRemovedFromChannel -= OnParticipantRemoved;
        }
    }

    private void OnParticipantAdded(VivoxParticipant participant)
    {
        Debug.Log($"Participant ajouté: {participant.PlayerId} (speechDetected={participant.SpeechDetected})");

        PlayerEntryUI PlayerEntryUIRef = _playerEntryUIList[_numberPlayerInGame];
        PlayerEntryUIRef.Setup(participant);

        _entries[participant.PlayerId] = PlayerEntryUIRef;

        _numberPlayerInGame++;
    }

    private void OnParticipantRemoved(VivoxParticipant participant)
    {
        Debug.Log($"Participant retiré: {participant.PlayerId}");
        if (_entries.TryGetValue(participant.PlayerId, out PlayerEntryUI PlayerEntryUIRef))
        {
            PlayerEntryUIRef.Cleanup();
            _entries.Remove(participant.PlayerId);

            _numberPlayerInGame--;
        }
    }
}
