using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayerManager : NetworkBehaviour
{
    private Dictionary<ulong, GameObject> _players = new();

    [SerializeField] private Transform _playerPrefab;
    [SerializeField] private List<Transform> _playerLocationAfterChoiceLaws;
    [SerializeField] private List<Transform> _spawnPlayer;

    public static SpawnPlayerManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn() //Seul l'host peut voir ça
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    /// <summary>
    /// On spawn les joueurs au 0 0
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadSceneMode"></param>
    /// <param name="clientsCompleted"></param>
    /// <param name="clientsTimedOut"></param>
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(_playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            _players[clientId] = playerTransform.gameObject;
        }

        SetupPlayersAtGameStart();
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestStartGameServerRpc(int spawnIndex, ServerRpcParams rpcParams = default) //Comme le network transform est réecris je dois demander au client de se tp car il est le seul à pouvoir le faire !
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        ClientRpcParams clientRpcParams = new ClientRpcParams  // Envoie un ClientRpc ciblé pour que le client se téléporte
        {
            Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { senderClientId } }
        };

        _players[senderClientId].GetComponent<PlayerMovement>().TeleportClientRpc(
            _playerLocationAfterChoiceLaws[spawnIndex].position,
            clientRpcParams
        );
        _players[senderClientId].GetComponent<PlayerMovement>().EnableControlsClientRpc(clientRpcParams);
    }


    private void SetupPlayersAtGameStart()
    {
        foreach (KeyValuePair<ulong, GameObject> kvp in _players)
        {
            ulong clientId = kvp.Key;
            GameObject player = kvp.Value;
            int playerIndex = LegalWarNetworkManager.Instance.GetPlayerDataFromClientId(clientId).playerIndex;

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { clientId } }
            };

            player.GetComponent<PlayerMovement>().TeleportClientRpc(_spawnPlayer[playerIndex].position, clientRpcParams);
            player.GetComponent<PlayerMovement>().DisableControlsClientRpc(clientRpcParams); // désactiver les inputs au début
            player.GetComponent<PoliticsClass>().ChangePlayerColorsUIClientRpc(clientRpcParams);
        }
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnClientSceneLoaded;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnClientSceneLoaded;
    }

    private async void OnClientSceneLoaded(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "LegalWar")
        {
            await VivoxManager.Instance.LeaveTestChannel();
            await VivoxManager.Instance.JoinProximityChannel();
        }
    }

}
