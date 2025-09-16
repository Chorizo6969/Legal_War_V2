using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        LegalWarNetworkManager.Instance.OnTryingToJoinGame += LegalWarNetworkManagement_OnTryingToJoinGame;
        LegalWarNetworkManager.Instance.OnFailedToJoinGame += LegalWarNetworkManagement_OnFailedToJoinGame;
        LegalWarNetworkManager.Instance.OnSucessJoinGame += LegalWar_JoinGameAgree;
        Hide();
    }

    private void LegalWar_JoinGameAgree(object sender, EventArgs e)
    {
        Hide();
    }

    private void LegalWarNetworkManagement_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LegalWarNetworkManagement_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LegalWarNetworkManager.Instance.OnTryingToJoinGame -= LegalWarNetworkManagement_OnTryingToJoinGame;
        LegalWarNetworkManager.Instance.OnFailedToJoinGame -= LegalWarNetworkManagement_OnFailedToJoinGame;
        LegalWarNetworkManager.Instance.OnSucessJoinGame -= LegalWar_JoinGameAgree;
    }
}
