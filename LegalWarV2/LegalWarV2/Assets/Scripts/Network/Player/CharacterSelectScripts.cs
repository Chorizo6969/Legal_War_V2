using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScripts : MonoBehaviour
{
    [SerializeField] private int _playerIndex = 0;
    [SerializeField] private GameObject _redayGameObject;
    [SerializeField] private PlayerVisual _playerVisual;
    //[SerializeField] private Button _kickButton;

    private void Awake()
    {
        //_kickButton.onClick.AddListener(() =>
        //{
        //    PlayerData playerdata = LegalWarNetworkManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
        //    LegalWarNetworkManager.Instance.KickPlayer(playerdata.clientID);
        //});
    }

    private void Start()
    {
        LegalWarNetworkManager.Instance.OnPlayerDataNetworkListChanged += LegalWarNetworkManagement_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChange += CharacterSelectReady_OnReadyChange;

        //_kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChange(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void LegalWarNetworkManagement_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (LegalWarNetworkManager.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();
            PlayerData playerdata = LegalWarNetworkManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            _redayGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerdata.clientID));
            _playerVisual.SetColorPlayer(LegalWarNetworkManager.Instance.GetPlayerColor(playerdata.meshID));
        }
        else
        {
            Hide();
        }

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LegalWarNetworkManager.Instance.OnPlayerDataNetworkListChanged -= LegalWarNetworkManagement_OnPlayerDataNetworkListChanged;
    }
}
