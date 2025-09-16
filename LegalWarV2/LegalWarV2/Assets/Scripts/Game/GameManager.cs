using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    [SerializeField] private List<Transform> _spawnPlayer;
    [SerializeField] private List<Transform> _startGame;

    private CharacterController _characterController;
    private PlayerMovement _playerMovement;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _characterController = Player.GetComponent<CharacterController>();
        _playerMovement= Player.GetComponent<PlayerMovement>();

        Player.transform.position = _spawnPlayer[0].position;
        _playerMovement.DisableControls();
    }

    public void StartGame()
    {
        _characterController.enabled = false;
        Player.transform.position = _startGame[0].position;
        _characterController.enabled = true;

        _playerMovement.EnableControls();
    }
}
