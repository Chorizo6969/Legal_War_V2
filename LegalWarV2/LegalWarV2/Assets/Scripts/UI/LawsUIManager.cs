using TMPro;
using Unity.Collections;
using UnityEngine;

public class LawsUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lawsText;

    public static LawsUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateLawsText(FixedString128Bytes NewText) => _lawsText.text = "Sujet choisi : " + NewText.ToString();
}
