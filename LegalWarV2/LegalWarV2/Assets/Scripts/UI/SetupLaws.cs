using TMPro;
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
            GameManager.Instance.StartGame();
            LawsManager.Instance.PlayerLawsList.Add(_inputFieldTest.text);
            _panelLaws.Hide();

            SetupMainLawsPanel.Instance.HideWaitingScreen();

            LawsManager.Instance.StartTirageLaws();
        });
    }
}
