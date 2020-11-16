using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenContainer : MonoBehaviour
{
    public static ScoreScreenContainer Instance; 
    public GameObject ScoreScreenPanel;

    [Space(10)]
    [Header("Text Labels")]
    public Text SubtitleLabel;

    public Text Player1Label;
    public Text Player2Label;
    public Text Player1MarkedTilesScoreLabel;
    public Text Player2MarkedTilesScoreLabel; 
    public Text Player1TimesCaughtScoreLabel;
    public Text Player2TimesCaughtScoreLabel; 
    public Text Player1TotalScoreLabel;
    public Text Player2TotalScoreLabel;

    public Text WaitingForNextLevelLabel;

    [Space(10)]
    public Button NextLevelButton;

    private ScoreCalculator _scoreCalculator;

    public void Awake()
    {
        Guard.CheckIsNull(ScoreScreenPanel, "ScoreScreenPanel", gameObject);

        Guard.CheckIsNull(SubtitleLabel, "SubtitleLabel", gameObject);
        Guard.CheckIsNull(Player1Label, "Player1Label", gameObject);
        Guard.CheckIsNull(Player2Label, "Player2Label", gameObject);
        Guard.CheckIsNull(Player1MarkedTilesScoreLabel, "Player1MarkedTilesScoreLabel", gameObject);
        Guard.CheckIsNull(Player2MarkedTilesScoreLabel, "Player2MarkedTilesScoreLabel", gameObject);
        Guard.CheckIsNull(Player1TimesCaughtScoreLabel, "Player1TimesCaughtScoreLabel", gameObject);
        Guard.CheckIsNull(Player2TimesCaughtScoreLabel, "Player2TimesCaughtScoreLabel", gameObject);
        Guard.CheckIsNull(Player2TotalScoreLabel, "Player2TotalScoreLabel", gameObject);
        Guard.CheckIsNull(Player2TotalScoreLabel, "Player2TotalScoreLabel", gameObject);
        Guard.CheckIsNull(WaitingForNextLevelLabel, "WaitingForNextLevelLabel", gameObject);
        Guard.CheckIsNull(NextLevelButton, "NextLevelButton", gameObject);

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

        SubtitleLabel.text = $"You escaped from {MazeLevelManager.Instance.Level.MazeName}";

        if (GameManager.Instance.GameType == GameType.SinglePlayer)
            ShowSingleplayerScore(_scoreCalculator.PlayerScores);
        else
            ShowMultiplayerScore(_scoreCalculator.PlayerScores);
    }

    private void ShowSingleplayerScore(Dictionary<PlayerNumber, PlayerScore> playerScores)
    {
        Player1Label.text = "Player 1";

        Player1MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player1].TileMarkScore.ToString();
        Player1TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player1].PlayerCaughtScore.ToString();
        Player1TotalScoreLabel.text = playerScores[PlayerNumber.Player1].TotalScore.ToString();

        Player1Label.gameObject.SetActive(true);
        Player2Label.gameObject.SetActive(false);

        Player1MarkedTilesScoreLabel.gameObject.SetActive(true);
        Player2MarkedTilesScoreLabel.gameObject.SetActive(false);
        Player1TimesCaughtScoreLabel.gameObject.SetActive(true);
        Player2TimesCaughtScoreLabel.gameObject.SetActive(false);
        Player1TotalScoreLabel.gameObject.SetActive(true);
        Player2TotalScoreLabel.gameObject.SetActive(false);

        NextLevelButton.gameObject.SetActive(true);

        OpenScoreScreenPanel();
    }

    private void ShowMultiplayerScore(Dictionary<PlayerNumber, PlayerScore> playerScores)
    {
        Player1Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player1].PhotonView.Owner?.NickName;
        Player2Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player2].PhotonView.Owner?.NickName;

        Player1MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player1].TileMarkScore.ToString();
        Player2MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player2].TileMarkScore.ToString();
        Player1TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player1].PlayerCaughtScore.ToString();
        Player2TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player2].PlayerCaughtScore.ToString();
        Player1TotalScoreLabel.text = playerScores[PlayerNumber.Player1].TotalScore.ToString();
        Player2TotalScoreLabel.text = playerScores[PlayerNumber.Player2].TotalScore.ToString();

        Player1Label.gameObject.SetActive(true);
        Player2Label.gameObject.SetActive(true);

        Player1MarkedTilesScoreLabel.gameObject.SetActive(true);
        Player2MarkedTilesScoreLabel.gameObject.SetActive(true);
        Player1TimesCaughtScoreLabel.gameObject.SetActive(true);
        Player2TimesCaughtScoreLabel.gameObject.SetActive(true);
        Player1TotalScoreLabel.gameObject.SetActive(true);
        Player2TotalScoreLabel.gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            NextLevelButton.gameObject.SetActive(true);
            WaitingForNextLevelLabel.gameObject.SetActive(false);
        }
        else
        {
            WaitingForNextLevelLabel.text = $"Waiting for {PhotonNetwork.MasterClient.NickName} to start the next level...";
            WaitingForNextLevelLabel.gameObject.SetActive(true);
            NextLevelButton.gameObject.SetActive(false);
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
