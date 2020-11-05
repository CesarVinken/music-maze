using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenContainer : MonoBehaviour
{
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

        ScoreScreenPanel.SetActive(false);
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;
    }

    public void OnMazeLevelCompleted()
    {
        ScoreManager.Instance.CalculateScores();

        SubtitleLabel.text = $"You escaped from {MazeLevelManager.Instance.Level.MazeName}";

        if (GameManager.Instance.GameType == GameType.SinglePlayer)
            ShowSingleplayerScore();
        else
            ShowMultiplayerScore();
    }

    private void ShowSingleplayerScore()
    {
        Player1Label.text = "Player 1";

        Player1MarkedTilesScoreLabel.text = ScoreManager.Instance.Player1Score.TileMarkScore.ToString();
        Player1TimesCaughtScoreLabel.text = ScoreManager.Instance.Player1Score.PlayerCaughtScore.ToString();
        Player1TotalScoreLabel.text = ScoreManager.Instance.Player1Score.TotalScore.ToString();

        Player1Label.gameObject.SetActive(true);
        Player2Label.gameObject.SetActive(false);

        Player1MarkedTilesScoreLabel.gameObject.SetActive(true);
        Player2MarkedTilesScoreLabel.gameObject.SetActive(false);
        Player1TimesCaughtScoreLabel.gameObject.SetActive(true);
        Player2TimesCaughtScoreLabel.gameObject.SetActive(false);
        Player1TotalScoreLabel.gameObject.SetActive(true);
        Player2TotalScoreLabel.gameObject.SetActive(false);

        NextLevelButton.gameObject.SetActive(true);

        ScoreScreenPanel.SetActive(true);
    }

    private void ShowMultiplayerScore()
    {
        Player1Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player1].PhotonView.Owner?.NickName;
        Player2Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player2].PhotonView.Owner?.NickName;

        Player1MarkedTilesScoreLabel.text = ScoreManager.Instance.Player1Score.TileMarkScore.ToString();
        Player2MarkedTilesScoreLabel.text = ScoreManager.Instance.Player2Score.TileMarkScore.ToString();
        Player1TimesCaughtScoreLabel.text = ScoreManager.Instance.Player1Score.PlayerCaughtScore.ToString();
        Player2TimesCaughtScoreLabel.text = ScoreManager.Instance.Player2Score.PlayerCaughtScore.ToString();
        Player1TotalScoreLabel.text = ScoreManager.Instance.Player1Score.TotalScore.ToString();
        Player2TotalScoreLabel.text = ScoreManager.Instance.Player2Score.TotalScore.ToString();

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
        }

        ScoreScreenPanel.SetActive(true);
    }

    public void NextLevel()
    {
        Logger.Log("Load next random level.");

        ScoreScreenPanel.SetActive(false);
    }
}
