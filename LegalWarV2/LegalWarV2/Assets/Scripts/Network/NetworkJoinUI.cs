using System;
using UnityEngine;
using UnityEngine.UI;
using static Loader.Scene;

public class NetworkJoinUi : MonoBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;

    private void Awake()
    {
        _hostButton.onClick.AddListener(() => {
            LegalWarNetworkManager.Instance.StartHost();
            //Loader.Load(SelectCharacterScene);
            HideWindow();
        });

        _clientButton.onClick.AddListener(() => {
            LegalWarNetworkManager.Instance.StartClient();
        });
    }

    private void Start()
    {
        LegalWarNetworkManager.Instance.OnSucessJoinGame += LegalWar_OnJoinGameAgree;
        LegalWarNetworkManager.Instance.OnFailedToJoinGame += LegalWar_OnFailedToJoinGame;
    }

    private void LegalWar_OnFailedToJoinGame(object sender, EventArgs e)
    {
        this.gameObject.SetActive(true);
    }

    private void LegalWar_OnJoinGameAgree(object sender, EventArgs e)
    {
        HideWindow();
    }

    private void HideWindow()
    {
        this.gameObject.SetActive(false);
    }
}
