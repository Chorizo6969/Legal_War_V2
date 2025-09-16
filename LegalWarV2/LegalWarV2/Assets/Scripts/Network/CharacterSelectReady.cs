using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Loader.Scene;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> _playerReadyDico;
    public static CharacterSelectReady Instance;

    public event EventHandler OnReadyChange;

    private void Awake()
    {
        Instance = this;
        _playerReadyDico = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRPC(serverRpcParams.Receive.SenderClientId);
        _playerReadyDico[serverRpcParams.Receive.SenderClientId] = true;

        bool allCLientsReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDico.ContainsKey(clientID) || !_playerReadyDico[clientID])
            {
                allCLientsReady = false; //Not Ready
                break;
            }
        }

        if (allCLientsReady)
        {
            Loader.Load(LegalWar);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRPC(ulong clientID)
    {
        _playerReadyDico[(clientID)] = true;
        OnReadyChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return _playerReadyDico.ContainsKey(clientID) && _playerReadyDico[clientID];
    }
}
