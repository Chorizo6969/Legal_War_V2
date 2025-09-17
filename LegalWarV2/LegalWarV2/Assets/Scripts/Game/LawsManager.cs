using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LawsManager : NetworkBehaviour
{
    public NetworkList<PlayerLaw> PlayerLawsList;

    public NetworkVariable<int> ChosenLawIndex = new NetworkVariable<int>(value: -1, writePerm: NetworkVariableWritePermission.Server); //Indique la loi tiré (-1 = null ref)

    [SerializeField] private GameObject _playerLawsUI;
    [SerializeField] private GameObject _loadingScreenUI;

    public static LawsManager Instance;

    private void Awake()
    {   
        Instance = this;
        PlayerLawsList = new NetworkList<PlayerLaw>();
    }

    public void StartTirageLaws()
    {
        if (ChosenLawIndex.Value >= 0 && ChosenLawIndex.Value < PlayerLawsList.Count) //Je ne tire la loi que lorsque tout les joueurs ont fini de proposer une loi
        {
            LawsUIManager.Instance.UpdateLawsText(PlayerLawsList[ChosenLawIndex.Value].law);
            _playerLawsUI.SetActive(true); //J'affiche le vrai panel de jeu
            _loadingScreenUI.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitLawServerRpc(string law, ServerRpcParams rpcParams = default) //Un joueur va envoyé la loi écrite ici
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        PlayerLawsList.Add(new PlayerLaw(senderClientId, law)); //Je la "save"

        if (PlayerLawsList.Count == NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            ChosenLawIndex.Value = Random.Range(0, PlayerLawsList.Count); //Je choisis une random
            UpdateLawsTextClientRpc(PlayerLawsList[ChosenLawIndex.Value].law); //J'affiche chez tout le monde le choix effectuer


            int PlayerIdColor = UpdateColorsUIPanel(PlayerLawsList[ChosenLawIndex.Value].law);
            ChangeUILawsPanel.Instance.ChangePlayerColorsUIClientRpc(PlayerIdColor);
        }
    }

    [ClientRpc]
    private void UpdateLawsTextClientRpc(FixedString128Bytes lawText)
    {
        LawsUIManager.Instance.UpdateLawsText(lawText);
        _playerLawsUI.SetActive(true);
        _loadingScreenUI.SetActive(false);
    }

    private int UpdateColorsUIPanel(FixedString128Bytes law)
    {
        ulong clientId = OriginePlayerLaws(law);
        PlayerData playerData = LegalWarNetworkManager.Instance.GetPlayerDataFromClientId(clientId);
        return playerData.meshID;
    }

    private ulong OriginePlayerLaws(FixedString128Bytes lawText) //Je cherche quel joueur à mis cette loi
    {
        for (int i = 0; i < PlayerLawsList.Count; i++)
        {
            if (PlayerLawsList[i].law == lawText)
            {
                return PlayerLawsList[i].clientID;
            }
        }
        return default;
    }
}
