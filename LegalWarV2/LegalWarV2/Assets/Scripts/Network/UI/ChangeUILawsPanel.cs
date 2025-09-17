using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUILawsPanel : NetworkBehaviour
{
    [Header("ID")]
    [SerializeField] private List<Sprite> _allPartyIdSprites = new(); //Photo
    [SerializeField] private Image _partyIdRef;

    [Header("Background ID")]
    [SerializeField] private List<Sprite> _allPartyBackgroundColor = new(); //Rond coloré
    [SerializeField] private Image _partyBackgroundColorRef;

    [Header("Party Name")]
    [SerializeField] private List<string> _allPartyName = new();
    [SerializeField] private TextMeshProUGUI _partyName; //Pseudo parti

    [Header("Panel Border")]
    [SerializeField] private List<Color> _allpartyBorderColorPanel = new(); //Border
    [SerializeField] private Image _partyBorderColorPanel;

    [Header("Panel Background")]
    [SerializeField] private List<Color> _allpartyColorPanel = new(); //Fond
    [SerializeField] private Image _partyBackgroundColorPanel;


    public static ChangeUILawsPanel Instance;


    private void Awake()
    {
        Instance = this;
    }

    [ClientRpc]
    public void ChangePlayerColorsUIClientRpc(int playerColorIndex, ClientRpcParams clientRpcParams = default) // MeshID == 0 c'est RN, 1 c'est RP, 2 c'est centre, etc...
    {
        _partyIdRef.sprite = _allPartyIdSprites[playerColorIndex];

        _partyBackgroundColorRef.sprite = _allPartyBackgroundColor[playerColorIndex];

        _partyName.text = _allPartyName[playerColorIndex];

        _partyBorderColorPanel.color = _allpartyBorderColorPanel[playerColorIndex
            ];
        _partyBackgroundColorPanel.color = _allpartyColorPanel[playerColorIndex];
    }
}
