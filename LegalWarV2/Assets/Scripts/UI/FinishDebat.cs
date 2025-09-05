using UnityEngine;
using UnityEngine.UI;

public class FinishDebat : MonoBehaviour
{
    [SerializeField] private Button _startVote;

    private void Awake()
    {
        _startVote.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
