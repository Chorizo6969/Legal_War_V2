using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayerManager : NetworkBehaviour
{
    [SerializeField] private Transform _playerPrefab;

    [SerializeField] private List<Transform> _spawnPlayer;
    [SerializeField] private List<Transform> _startGame;

    private List<GameObject> _playerList = new();

    public static SpawnPlayerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
        {
            ulong cliendId = NetworkManager.Singleton.ConnectedClientsIds[i];

            Transform playerTransform = Instantiate(_playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(cliendId, true);

            _playerList.Add(playerTransform.gameObject);

            playerTransform.position = _spawnPlayer[i].position;
            playerTransform.GetComponent<PlayerMovement>().DisableControls();
        }
    }

    public void StartGame()
    {
        //for (int i = 0; i <  _playerList.Count; i++)
        //{
        //    CharacterController PlayerCharacterController = _playerList[i].GetComponent<CharacterController>();

        //    PlayerCharacterController.enabled = false;
        //    _playerList[i].transform.position = _startGame[i].position;
        //    PlayerCharacterController.enabled = true;

        //    _playerList[i].GetComponent<PlayerMovement>().EnableControls();
        //}
    }
}
