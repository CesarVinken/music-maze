using Character;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MainScreenOverlayCanvas : MonoBehaviour
    {
        public static MainScreenOverlayCanvas Instance;

        public GameObject ConsoleContainer;
        public GameObject ConsoleButton;

        [SerializeField] private GameObject _blackOutSquarePrefab;
        [SerializeField] private GameObject _gameEditorUIPrefab;

        [SerializeField] private GameObject _playerZeroOptionMessagePrefab;
        [SerializeField] private GameObject _playerOneOptionMessagePrefab;

        public List<BlackOutSquare> BlackOutSquares;
        public List<PlayerMessagePanel> OpenMessagePanels = new List<PlayerMessagePanel>();

        public void Awake()
        {
            Instance = this;

            Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer", gameObject);
            Guard.CheckIsNull(ConsoleButton, "ConsoleButton", gameObject);

            Guard.CheckIsNull(_blackOutSquarePrefab, "BlackOutSquarePrefab", gameObject);
            Guard.CheckIsNull(_gameEditorUIPrefab, "GameEditorUIPrefab", gameObject);

            Guard.CheckIsNull(_playerZeroOptionMessagePrefab, "_playerZeroOptionMessagePrefab", gameObject);
            Guard.CheckIsNull(_playerOneOptionMessagePrefab, "_playerOneOptionMessagePrefab", gameObject);

            SetupBlackOutSquares();

            if (PersistentGameManager.CurrentPlatform == Platform.Android)
            {
                ConsoleButton.SetActive(true);
            }
        }

        public void Start()
        {
            if (EditorCanvasUI.Instance == null)
            {
                GameObject editorUI = Instantiate(_gameEditorUIPrefab);
                editorUI.SetActive(false);
            }
            else
            {
                EditorCanvasUI.Instance.UpdateCanvasForSceneChange();
            }
        }

        private void SetupBlackOutSquares()
        {
            int blackOutSquaresToInstantiate = GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer ? 2 : 1;

            for (int i = 0; i < blackOutSquaresToInstantiate; i++)
            {
                GameObject blackOutSquareGo = Instantiate(_blackOutSquarePrefab, transform);
                BlackOutSquare blackOutSquare = blackOutSquareGo.GetComponent<BlackOutSquare>();

                if (blackOutSquare == null)
                {
                    Logger.Error("Could not find black out square component on BlackOutSquare GameObject");
                }

                BlackOutSquares.Add(blackOutSquare);
            }

            if (BlackOutSquares.Count == 2)
            {
                float fullCanvasWidth = 1280;

                RectTransform rt1 = BlackOutSquares[0].GetComponent<RectTransform>();
                RectTransform rt2 = BlackOutSquares[1].GetComponent<RectTransform>();

                rt1.offsetMax = new Vector2(fullCanvasWidth / -2, rt2.offsetMin.y);
                rt2.offsetMin = new Vector2(fullCanvasWidth / 2, rt2.offsetMin.y);
            }

        }

        public void BlackOutSquaresToClear()
        {
            for (int i = 0; i < BlackOutSquares.Count; i++)
            {
                IEnumerator clearBlackOutSquareCoroutine = BlackOutSquares[i].ToClear();

                StartCoroutine(clearBlackOutSquareCoroutine);
            }
        }

        public void BlackOutSquaresToBlack()
        {
            for (int i = 0; i < BlackOutSquares.Count; i++)
            {
                IEnumerator darkenBlackOutSquareCoroutine = BlackOutSquares[i].ToBlack();

                StartCoroutine(darkenBlackOutSquareCoroutine);
            }
        }

        public void ResetBlackOutSquares()
        {
            for (int i = 0; i < BlackOutSquares.Count; i++)
            {
                BlackOutSquares[i].ResetToDefault();
            }
        }

        public void ShowPlayerZeroOptionMessagePanel(string message, PlayerNumber playerNumber = PlayerNumber.Player1)
        {
            GameObject playerMessagePanelGO = Instantiate(_playerZeroOptionMessagePrefab, transform);
            PlayerZeroOptionMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerZeroOptionMessagePanel>();
            playerMessagePanel.Initialise(message, playerNumber);
        }

        public void ShowPlayerOneOptionMessagePanel(string message, string buttonAText, GameUIAction gameUIAction, PlayerNumber playerNumber = PlayerNumber.Player1)
        {
            GameObject playerMessagePanelGO = Instantiate(_playerOneOptionMessagePrefab, transform);
            PlayerOneOptionMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerOneOptionMessagePanel>();
            playerMessagePanel.Initialise(message, buttonAText, gameUIAction, playerNumber);
        }

        //public void ClosePlayerMessagePanel()
        //{
        //    PlayerMessagePanel.Instance.CloseMessagePanel();
        //}

        //public void ShowPlayerWarning(string message)
        //{
        //    GameObject playerMessagePanelGO = Instantiate(_playerOneOptionMessagePrefab, transform);
        //    PlayerMessagePanel playerMessagePanel = playerMessagePanelGO.GetComponent<PlayerMessagePanel>();
        //    playerMessagePanel.Initialise(message, GameUIAction.ClosePanel, PlayerNumber.Player1);
        //}
    }
}