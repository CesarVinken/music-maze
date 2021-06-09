using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenOverlayCanvas : MonoBehaviour
{
    public static MainScreenOverlayCanvas Instance;

    public GameObject ConsoleContainer;
    public GameObject ConsoleButton;

    [SerializeField] private GameObject _blackOutSquarePrefab;
    [SerializeField] private GameObject _gameEditorUIPrefab;

    public List<BlackOutSquare> BlackOutSquares;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer", gameObject);
        Guard.CheckIsNull(ConsoleButton, "ConsoleButton", gameObject);

        Guard.CheckIsNull(_blackOutSquarePrefab, "BlackOutSquarePrefab", gameObject);
        Guard.CheckIsNull(_gameEditorUIPrefab, "GameEditorUIPrefab", gameObject);

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
            GameObject blackOutSquareGo = GameObject.Instantiate(_blackOutSquarePrefab, transform);
            BlackOutSquare blackOutSquare = blackOutSquareGo.GetComponent<BlackOutSquare>();

            if (blackOutSquare == null)
            {
                Logger.Error("Could not find black out square component on BlackOutSquare GameObject");
            }

            BlackOutSquares.Add(blackOutSquare);
        }

        if(BlackOutSquares.Count == 2)
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

    public void ResetBlackOutSquares()
    {
        for (int i = 0; i < BlackOutSquares.Count; i++)
        {
            BlackOutSquares[i].ResetToDefault();
        }
    }
}
