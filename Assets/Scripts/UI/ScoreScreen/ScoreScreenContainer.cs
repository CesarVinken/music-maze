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
        Guard.CheckIsNull(Player1TotalScoreLabel, "Player1TotalScoreLabel", gameObject);
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
        SubtitleLabel.text = $"You escaped from {MazeLevelManager.Instance.Level.MazeName}";

        if (GameManager.Instance.GameType == GameType.SinglePlayer)
            ShowSingleplayerScore();
        else
            ShowMultiplayerScore();
    }

    private void ShowSingleplayerScore()
    {
        Player1Label.text = "Player 1";

        Player1Label.gameObject.SetActive(true);

        NextLevelButton.gameObject.SetActive(true);

        ScoreScreenPanel.SetActive(true);
    }

    private void ShowMultiplayerScore()
    {
        Player1Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player1].PhotonView.Owner?.NickName;
        Player2Label.text = CharacterManager.Instance.MazePlayers[PlayerNumber.Player2].PhotonView.Owner?.NickName;

        Player1Label.gameObject.SetActive(true);
        Player2Label.gameObject.SetActive(true);

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
