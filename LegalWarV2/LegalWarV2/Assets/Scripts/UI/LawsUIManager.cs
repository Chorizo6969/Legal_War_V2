using TMPro;
using UnityEngine;

public class LawsUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lawsText;

    public static LawsUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateLawsText(string NewText) => _lawsText.text = "Sujet choisi : " + NewText;
}
