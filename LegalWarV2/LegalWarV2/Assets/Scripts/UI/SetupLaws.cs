using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SetupLaws : MonoBehaviour
{
    [SerializeField] private Button _validateButton;
    [SerializeField] private PanelLaws _panelLaws;
    [SerializeField] private TMP_InputField _inputFieldTest;

    private void Awake()
    {
        _validateButton.onClick.AddListener(() =>
        {
            int playerIndex = FindPlayerIndex();
            SpawnPlayerManager.Instance.RequestStartGameServerRpc(playerIndex);

            LawsManager.Instance.SubmitLawServerRpc(_inputFieldTest.text);
            _panelLaws.Hide();
        });

    }

    private int FindPlayerIndex()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        PlayerData data = LegalWarNetworkManager.Instance.GetPlayerDataFromClientId(localClientId);
        return data.playerIndex;
    }
}
