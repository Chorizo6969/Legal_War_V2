using UnityEngine;

public class SetupMainLawsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _waitingPanel;
    [SerializeField] private GameObject _lawsPanel;

    public static SetupMainLawsPanel Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowWaitingScreen();
    }

    public void HideWaitingScreen()
    {
        _waitingPanel.SetActive(false);
        _lawsPanel.SetActive(true);
    }

    public void ShowWaitingScreen()
    {
        _waitingPanel.SetActive(true);
        _lawsPanel.SetActive(false);
    }
}
