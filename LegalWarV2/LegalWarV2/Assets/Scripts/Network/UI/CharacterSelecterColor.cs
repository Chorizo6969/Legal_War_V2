using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColorUI : MonoBehaviour
{
    [SerializeField] private int colorID;
    [SerializeField] private GameObject _selectedGameObject;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => {
            LegalWarNetworkManager.Instance.ChangePlayerColor(colorID);
        });
    }

    private void Start()
    {
        LegalWarNetworkManager.Instance.OnPlayerDataNetworkListChanged += LegalWarNetworkManagement_OnPlayerDataNetworkListChanged;
        UpdateIsSelected();
    }

    private void LegalWarNetworkManagement_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (LegalWarNetworkManager.Instance.GetPlayerData().meshID == colorID)
            _selectedGameObject.SetActive(true);
        else
            _selectedGameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LegalWarNetworkManager.Instance.OnPlayerDataNetworkListChanged -= LegalWarNetworkManagement_OnPlayerDataNetworkListChanged;
    }
}
