using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PoliticsClass : NetworkBehaviour
{
    [SerializeField] private List<Sprite> _allPartyColorsUISprites = new();
    [SerializeField] private List<string> _allPartyName = new();
    [SerializeField] private Image _partyColorsUIRef;
    [SerializeField] private TextMeshProUGUI _partyName;
    [SerializeField] private Image _partyScoringBackGroundRef;

    [ClientRpc]
    public void ChangePlayerColorsUIClientRpc(ClientRpcParams clientRpcParams = default) // MeshID == 0 c'est RN, 1 c'est RP, 2 c'est centre, etc...
    {
        if (!IsOwner) return;
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        int playerColorIndex = LegalWarNetworkManager.Instance.GetPlayerDataFromClientId(localClientId).meshID;

        _partyColorsUIRef.sprite = _allPartyColorsUISprites[playerColorIndex];
        _partyScoringBackGroundRef.sprite = _allPartyColorsUISprites[playerColorIndex];
        _partyName.text = _allPartyName[playerColorIndex];
    }
}
