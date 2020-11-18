﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenContainer : MonoBehaviour
{
    public static ScoreScreenContainer Instance; 
    public GameObject ScoreScreenPanel;

    [Space(10)]
    [Header("Text Labels")]
    [SerializeField] private Text _subtitleLabel;

    [SerializeField] private Text _player1Label;
    [SerializeField] private Text _player2Label;
    [SerializeField] private Text _player1MarkedTilesScoreLabel;
    [SerializeField] private Text _player2MarkedTilesScoreLabel;
    [SerializeField] private Text _player1TimesCaughtScoreLabel;
    [SerializeField] private Text _player2TimesCaughtScoreLabel;
    [SerializeField] private Text _player1TotalScoreLabel;
    [SerializeField] private Text _player2TotalScoreLabel;

    [SerializeField] private Text _waitingForNextLevelLabel;

    [Space(10)]
    [SerializeField] private Button _nextLevelButton;

    private ScoreCalculator _scoreCalculator;

    public void Awake()
    {
        Guard.CheckIsNull(ScoreScreenPanel, "ScoreScreenPanel", gameObject);

        Guard.CheckIsNull(_subtitleLabel, "SubtitleLabel", gameObject);
        Guard.CheckIsNull(_player1Label, "Player1Label", gameObject);
        Guard.CheckIsNull(_player2Label, "Player2Label", gameObject);
        Guard.CheckIsNull(_player1MarkedTilesScoreLabel, "Player1MarkedTilesScoreLabel", gameObject);
        Guard.CheckIsNull(_player2MarkedTilesScoreLabel, "Player2MarkedTilesScoreLabel", gameObject);
        Guard.CheckIsNull(_player1TimesCaughtScoreLabel, "Player1TimesCaughtScoreLabel", gameObject);
        Guard.CheckIsNull(_player2TimesCaughtScoreLabel, "Player2TimesCaughtScoreLabel", gameObject);
        Guard.CheckIsNull(_player2TotalScoreLabel, "Player2TotalScoreLabel", gameObject);
        Guard.CheckIsNull(_player2TotalScoreLabel, "Player2TotalScoreLabel", gameObject);
        Guard.CheckIsNull(_waitingForNextLevelLabel, "WaitingForNextLevelLabel", gameObject);
        Guard.CheckIsNull(_nextLevelButton, "NextLevelButton", gameObject);

        _scoreCalculator = new ScoreCalculator();

        CloseScoreScreenPanel();

        Instance = this;
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;
    }

    public void OnMazeLevelCompleted()
    {
        _scoreCalculator.ResetMazeLevelScore();
        _scoreCalculator.CalculateScores();

        _subtitleLabel.text = $"You escaped from {MazeLevelManager.Instance.Level.MazeName}";

        if (GameManager.Instance.GameType == GameType.SinglePlayer)
            ShowSingleplayerScore(_scoreCalculator.PlayerScores);
        else
            ShowMultiplayerScore(_scoreCalculator.PlayerScores);
    }

    private void ShowSingleplayerScore(Dictionary<PlayerNumber, PlayerScore> playerScores)
    {
        _player1Label.text = "Player 1";

        _player1MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player1].TileMarkScore.ToString();
        _player1TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player1].PlayerCaughtScore.ToString();
        _player1TotalScoreLabel.text = playerScores[PlayerNumber.Player1].TotalScore.ToString();

        _player1Label.gameObject.SetActive(true);
        _player2Label.gameObject.SetActive(false);

        _player1MarkedTilesScoreLabel.gameObject.SetActive(true);
        _player2MarkedTilesScoreLabel.gameObject.SetActive(false);
        _player1TimesCaughtScoreLabel.gameObject.SetActive(true);
        _player2TimesCaughtScoreLabel.gameObject.SetActive(false);
        _player1TotalScoreLabel.gameObject.SetActive(true);
        _player2TotalScoreLabel.gameObject.SetActive(false);

        _nextLevelButton.gameObject.SetActive(true);

        OpenScoreScreenPanel();
    }

    private void ShowMultiplayerScore(Dictionary<PlayerNumber, PlayerScore> playerScores)
    {
        _player1Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player1].PhotonView.Owner?.NickName;
        _player2Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player2].PhotonView.Owner?.NickName;

        _player1MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player1].TileMarkScore.ToString();
        _player2MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player2].TileMarkScore.ToString();
        _player1TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player1].PlayerCaughtScore.ToString();
        _player2TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player2].PlayerCaughtScore.ToString();
        _player1TotalScoreLabel.text = playerScores[PlayerNumber.Player1].TotalScore.ToString();
        _player2TotalScoreLabel.text = playerScores[PlayerNumber.Player2].TotalScore.ToString();

        _player1Label.gameObject.SetActive(true);
        _player2Label.gameObject.SetActive(true);

        _player1MarkedTilesScoreLabel.gameObject.SetActive(true);
        _player2MarkedTilesScoreLabel.gameObject.SetActive(true);
        _player1TimesCaughtScoreLabel.gameObject.SetActive(true);
        _player2TimesCaughtScoreLabel.gameObject.SetActive(true);
        _player1TotalScoreLabel.gameObject.SetActive(true);
        _player2TotalScoreLabel.gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            _nextLevelButton.gameObject.SetActive(true);
            _waitingForNextLevelLabel.gameObject.SetActive(false);
        }
        else
        {
            _waitingForNextLevelLabel.text = $"Waiting for {PhotonNetwork.MasterClient.NickName} to start the next level...";
            _waitingForNextLevelLabel.gameObject.SetActive(true);
            _nextLevelButton.gameObject.SetActive(false);
        }

        OpenScoreScreenPanel();
    }

    public void NextLevel()
    {
        // pick a random level from the list of playable levels
        if(GameManager.Instance.PlayableLevelNames.Count == 0) // there are no levels left to choose from, reload all random levels.
        {
            Logger.Warning("We played all playable levels. Starting the random selection from the beginning");
            GameManager.Instance.PlayableLevelNames = MazeLevelLoader.GetAllPlayableLevelNames();
        }

        int randomIndex = Random.Range(0, GameManager.Instance.PlayableLevelNames.Count);
        string pickedLevel = GameManager.Instance.PlayableLevelNames[randomIndex];
        Logger.Log($"Load next random level: {pickedLevel}");

        MazeLevelManager.Instance.LoadNextLevel(pickedLevel); // triggers load next level event for both players
    }

    public void OpenScoreScreenPanel()
    {
        ScoreScreenPanel.SetActive(true);
    }

    public void CloseScoreScreenPanel()
    {
        ScoreScreenPanel.SetActive(false);
    }
}
