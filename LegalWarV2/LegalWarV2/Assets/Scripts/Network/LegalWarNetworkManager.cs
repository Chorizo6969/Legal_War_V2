using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LegalWarNetworkManager : NetworkBehaviour
{
    public static LegalWarNetworkManager Instance;
    private const int _maxPlayerCount = 5;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler OnSucessJoinGame;


    [SerializeField] private List<Mesh> _playerMeshList;
    [SerializeField] private TMP_InputField _code;
    [SerializeField] private TextMeshProUGUI _codeSection;

    private NetworkList<PlayerData> _playerDataNetworkList;

    private bool _clientDisconnectSubscribed = false;
    private string _joinCodeInput;

    public bool IncorectCode;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public async void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        string joinCode = await StartHostWithRelay(_maxPlayerCount, "default");
        Debug.Log($"Host started, join code: {joinCode}");
    }

    public async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        _codeSection.text = "Code : " + joinCode;

        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = _playerDataNetworkList[i];
            if (playerData.clientID == clientId)
            {
                _playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData
        {
            clientID = clientId,
            meshID = GetFirstUnusedColorId(),
            playerIndex = _playerDataNetworkList.Count
        });
    }

    public void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.Lobby.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "La partie est déjà en cours !";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= _maxPlayerCount)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Le nombre maximum de joueur est atteint !";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public async void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        if (!_clientDisconnectSubscribed)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
            _clientDisconnectSubscribed = true;
        }

        _joinCodeInput = _code.text;

        if (string.IsNullOrEmpty(_joinCodeInput) || !_relayCodeRegex.IsMatch(_joinCodeInput))
        {
            IncorectCode = true;
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
            return;
        }

        bool success = await StartClientWithRelay(_joinCodeInput, "default");
        if (success)
            OnSucessJoinGame?.Invoke(this, EventArgs.Empty);
        else
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }


    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            if (playerData.clientID == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public int GetPlayerDataIndexClientId(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].clientID == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    public Mesh GetPlayerColor(int ColorId)
    {
        return _playerMeshList[ColorId];
    }

    public void ChangePlayerColor(int ColorID)
    {
        ChangePlayerColorServerRPC(ColorID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRPC(int ColorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(ColorId))
            return;
        int playerDataIndex = GetPlayerDataIndexClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];
        playerData.meshID = ColorId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int ColorId)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            if (playerData.meshID == ColorId)
            {
                //Utilisé actullement
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < _playerMeshList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    private static readonly System.Text.RegularExpressions.Regex _relayCodeRegex =
        new System.Text.RegularExpressions.Regex("^[6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw]{6,12}$"); // Regex extraite de la doc officielle / erreur Relay

    public async Task<bool> StartClientWithRelay(string joinCode, string connectionType) //Tiré de la doc officielle de Relay, Choppe les possibles erreurs lors de la saisie du code
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        try
        {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            _codeSection.text = "Code : " + joinCode;

            return NetworkManager.Singleton.StartClient();
        }
        catch
        {
            IncorectCode = true;
            OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
            return false;
        }
    }
}
