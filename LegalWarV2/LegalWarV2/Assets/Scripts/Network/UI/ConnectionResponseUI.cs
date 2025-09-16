using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _message;
    [SerializeField] private Button _cancelButton;

    private void Awake()
    {
        _cancelButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        LegalWarNetworkManager.Instance.OnFailedToJoinGame += FPSNetworkManagement_OnFailedToJoinGame;
        Hide();
    }

    private void FPSNetworkManagement_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Show();
        _message.text = NetworkManager.Singleton.DisconnectReason;
        if (_message.text == "")
        {
            _message.text = "Échec de la conexion...";
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
        LegalWarNetworkManager.Instance.OnFailedToJoinGame -= FPSNetworkManagement_OnFailedToJoinGame;
    }
}
