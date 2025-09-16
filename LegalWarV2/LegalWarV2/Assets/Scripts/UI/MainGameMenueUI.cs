using UnityEngine;
using UnityEngine.UI;

public class MainGameMenueUI : MonoBehaviour
{

    [SerializeField] private Button _startGame;
    [SerializeField] private Button _quitGame;

    private void Awake()
    {
        _startGame.onClick.AddListener(() =>
        {
            Loader.StartGame();
        });

        _quitGame.onClick.AddListener(() =>
        {
            Loader.Quit();
        });
    }
}
