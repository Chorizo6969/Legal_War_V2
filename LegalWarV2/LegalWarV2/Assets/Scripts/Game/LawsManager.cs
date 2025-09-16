using System.Collections.Generic;
using UnityEngine;

public class LawsManager : MonoBehaviour
{
    public List<string> PlayerLawsList = new();

    [SerializeField] private GameObject _playerLawsUI;

    public static LawsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartTirageLaws()
    {
        int RandomIndex = Random.Range(0, PlayerLawsList.Count);
        LawsUIManager.Instance.UpdateLawsText(PlayerLawsList[RandomIndex]);
        _playerLawsUI.gameObject.SetActive(true);
    }
}
