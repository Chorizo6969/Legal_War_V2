using System;
using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private int _numberPlayers = 1;
    [SerializeField] private int _votantPour;

    public static PlayerScoreManager Instance;
    //public event EventHandler OnScoreChanged;


    private void Awake()
    {
        Instance = this;
    }

    private bool IsMajorite() => _votantPour >= (_numberPlayers / 2) + 1;
}
