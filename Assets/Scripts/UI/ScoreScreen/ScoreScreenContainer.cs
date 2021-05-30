using Photon.Pun;
using System.Collections;
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
    [SerializeField] private GameObject _toNextLevelButton;
    [SerializeField] private GameObject _toOverworldButton;

    private ScoreCalculator _scoreCalculator;
    private bool _screenIsOpen = false;

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
        Guard.CheckIsNull(_toNextLevelButton, "ToNextLevelButton", gameObject);
        Guard.CheckIsNull(_toOverworldButton, "ToOverworldButton", gameObject);

        _scoreCalculator = new ScoreCalculator();

        CloseScoreScreenPanel();

        Instance = this;
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;
    }

    public void Update()
    {
        if (PersistentGameManager.CurrentPlatform == Platform.PC && _screenIsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (_toNextLevelButton.activeSelf)
                {
                    ToNextLevel();
                }
                else if (_toOverworldButton.activeSelf)
                {
                    ToOverworld();
                }
            }
        }
    }

    public void OnMazeLevelCompleted()
    {
        // wait a moment so that the player can see exit animation before bringing up the scores.
        IEnumerator coroutine = OnMazeLevelCompletedCoroutine();
        StartCoroutine(coroutine);
    }

    private IEnumerator OnMazeLevelCompletedCoroutine()
    {
        float waitTime = 1.5f;

        yield return new WaitForSeconds(waitTime);

        _scoreCalculator.ResetMazeLevelScore();
        _scoreCalculator.CalculateScores();

        _subtitleLabel.text = $"You escaped from {MazeLevelManager.Instance.Level.Name}";

        ShowScore(_scoreCalculator.PlayerScores);
    }

    private void ShowScore(Dictionary<PlayerNumber, PlayerScore> playerScores)
    {
        foreach(KeyValuePair<PlayerNumber, PlayerScore> scoreSet in playerScores)
        {
            if(scoreSet.Key == PlayerNumber.Player1)
            {
                _player1Label.text = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(PlayerNumber.Player1).name;
                _player1MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player1].TileMarkScore.ToString();
                _player1TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player1].PlayerCaughtScore.ToString();
                _player1TotalScoreLabel.text = playerScores[PlayerNumber.Player1].TotalScore.ToString();
                _player1Label.gameObject.SetActive(true);
                _player1MarkedTilesScoreLabel.gameObject.SetActive(true);
                _player1TimesCaughtScoreLabel.gameObject.SetActive(true);
                _player1TotalScoreLabel.gameObject.SetActive(true);
            }
            else
            {
                _player2Label.text = GameManager.Instance.CharacterManager.GetPlayerCharacter<PlayerCharacter>(PlayerNumber.Player2).name;
                _player2MarkedTilesScoreLabel.text = playerScores[PlayerNumber.Player2].TileMarkScore.ToString();
                _player2TimesCaughtScoreLabel.text = playerScores[PlayerNumber.Player2].PlayerCaughtScore.ToString();
                _player2TotalScoreLabel.text = playerScores[PlayerNumber.Player2].TotalScore.ToString();
                _player2Label.gameObject.SetActive(true);
                _player2MarkedTilesScoreLabel.gameObject.SetActive(true);
                _player2TimesCaughtScoreLabel.gameObject.SetActive(true);
                _player2TotalScoreLabel.gameObject.SetActive(true);
            }
        }

        if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer ||
            GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ||
            (GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer && PhotonNetwork.IsMasterClient))
        {
            if (GameRules.GameMode == GameMode.Campaign)
            {
                _toNextLevelButton.SetActive(false);
                _toOverworldButton.SetActive(true);
            }
            else if (GameRules.GameMode == GameMode.RandomMaze)
            {
                _toNextLevelButton.SetActive(true);
                _toOverworldButton.SetActive(false);
            }
            _waitingForNextLevelLabel.gameObject.SetActive(false);
        }
        else
        {
            _waitingForNextLevelLabel.text = $"Waiting for {PhotonNetwork.MasterClient.NickName} to start the next level...";
            _waitingForNextLevelLabel.gameObject.SetActive(true);

            _toNextLevelButton.SetActive(false);
            _toOverworldButton.SetActive(false);
        }

        OpenScoreScreenPanel();
    }

    public void ToOverworld()
    {
        _screenIsOpen = false;

        string overworldName = "default";
        MazeLevelManager.Instance.LoadOverworld(overworldName);
    }

    public void ToNextLevel()
    {
        _screenIsOpen = false;

        // pick a random level from the list of playable levels
        if (GameManager.Instance.PlayableLevelNames.Count == 0) // there are no levels left to choose from, reload all random levels.
        {
            Logger.Warning("We played all playable levels. Starting the random selection from the beginning");
            GameManager.Instance.PlayableLevelNames = MazeLevelLoader.GetAllPlayableLevelNames();
        }
        Logger.Log("number of found levels: " + GameManager.Instance.PlayableLevelNames.Count);
        int randomIndex = Random.Range(0, GameManager.Instance.PlayableLevelNames.Count);
        string pickedLevel = GameManager.Instance.PlayableLevelNames[randomIndex];
        Logger.Log($"Load next random level: {pickedLevel}");

        MazeLevelManager.Instance.LoadNextLevel(pickedLevel); // triggers load next level event for both players
    }

    public void OpenScoreScreenPanel()
    {
        _screenIsOpen = true;
        ScoreScreenPanel.SetActive(true);
    }

    public void CloseScoreScreenPanel()
    {
        _screenIsOpen = false;
        ScoreScreenPanel.SetActive(false);
    }
}
