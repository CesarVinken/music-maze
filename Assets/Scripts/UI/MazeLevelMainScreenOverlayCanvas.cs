using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MazeLevelMainScreenOverlayCanvas : MonoBehaviour
    {
        public static MazeLevelMainScreenOverlayCanvas Instance;

        [SerializeField] private GameObject _exitsAreOpenMessagePrefab;
        [SerializeField] private GameObject _countdownTimerPrefab;

        private List<GameObject> _onScreenLabelGOs = new List<GameObject>();


        public void Awake()
        {
            Guard.CheckIsNull(_exitsAreOpenMessagePrefab, "_exitsAreOpenMessagePrefab", gameObject);
            Guard.CheckIsNull(_countdownTimerPrefab, "_countdownTimerPrefab", gameObject);

            Instance = this;
        }

        public void Start()
        {
            MazeLevelGameplayManager.Instance.AllPathsAreMarkedEvent += OnAllPathsAreMarked;
        }

        public void OnAllPathsAreMarked()
        {
            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer || GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                Vector3 spawnPosition = ScreenCalculator.GetScreenMiddle();

                IEnumerator spawnExitsAreOpenMessageRoutine = SpawnExitsAreOpenMessage(spawnPosition);
                StartCoroutine(spawnExitsAreOpenMessageRoutine);
            }
            else // split screen requires two texts
            {
                Vector3 spawnPosition1 = ScreenCalculator.GetSplitScreenScreen1Middle();
                Vector3 spawnPosition2 = ScreenCalculator.GetSplitScreenScreen2Middle();

                IEnumerator spawnExitsAreOpenMessage1Routine = SpawnExitsAreOpenMessage(spawnPosition1);
                IEnumerator spawnExitsAreOpenMessage2Routine = SpawnExitsAreOpenMessage(spawnPosition2);

                StartCoroutine(spawnExitsAreOpenMessage1Routine);
                StartCoroutine(spawnExitsAreOpenMessage2Routine);
            }
        }

        // Countdown is only triggered in the maze levels, not in the overworld
        public void SpawnCountdownTimer()
        {
            GameObject countdownTimerGO = Instantiate(_countdownTimerPrefab, transform);
            _onScreenLabelGOs.Add(countdownTimerGO);

            CountdownTimerUI countdownTimer = countdownTimerGO.GetComponent<CountdownTimerUI>();
            countdownTimer.SetText("Waiting..");


            if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer || GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
            {
                Vector3 spawnPosition = ScreenCalculator.GetScreenMiddle();
                countdownTimerGO.transform.position = spawnPosition;
                
                IEnumerator countdownTimerRoutine = CountDownRoutine(countdownTimer, 1);
                StartCoroutine(countdownTimerRoutine);
            }
            else // split screen requires two texts
            {
                GameObject countdownTimer2GO = Instantiate(_countdownTimerPrefab, transform);

                CountdownTimerUI countdownTimer2 = countdownTimer2GO.GetComponent<CountdownTimerUI>();
                countdownTimer2.SetText("Waiting..");

                Vector3 spawnPosition1 = ScreenCalculator.GetSplitScreenScreen1Middle();
                Vector3 spawnPosition2 = ScreenCalculator.GetSplitScreenScreen2Middle();
                countdownTimerGO.transform.position = spawnPosition1;
                countdownTimer2GO.transform.position = spawnPosition2;

                IEnumerator countdownTimerRoutine1 = CountDownRoutine(countdownTimer, 1);
                IEnumerator countdownTimerRoutine2 = CountDownRoutine(countdownTimer2, 2);

                StartCoroutine(countdownTimerRoutine1);
                StartCoroutine(countdownTimerRoutine2);
            }
        }

        public IEnumerator CountDownRoutine(CountdownTimerUI countdownTimerUI, int screenNo)
        {
            countdownTimerUI.gameObject.SetActive(true);
            int expectedNoOfPlayers = GameRules.GamePlayerType == GamePlayerType.SinglePlayer ? 1 : 2;

            while (MainScreenOverlayCanvas.Instance.BlackOutSquares[screenNo - 1].BlackStatus != BlackStatus.Clear ||
                GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().Count < expectedNoOfPlayers)
            {
                yield return new WaitForSeconds(0.4f);
            }
            countdownTimerUI.SetText("3");
            yield return new WaitForSeconds(1);
            countdownTimerUI.SetText("2");
            yield return new WaitForSeconds(1);
            countdownTimerUI.SetText("1");
            yield return new WaitForSeconds(1);

            // unfreeze enemies and player characters
            GameManager.Instance.CharacterManager.UnfreezeCharacters();

            _onScreenLabelGOs.Remove(countdownTimerUI.gameObject);
            Destroy(countdownTimerUI.gameObject);
        }

        public IEnumerator SpawnExitsAreOpenMessage(Vector3 spawnPosition)
        {
            GameObject exitsAreOpenMessageGO = Instantiate(_exitsAreOpenMessagePrefab, transform);
            exitsAreOpenMessageGO.transform.position = spawnPosition;
            _onScreenLabelGOs.Add(exitsAreOpenMessageGO);

            Text exitsAreOpenMessageText = exitsAreOpenMessageGO.GetComponent<Text>();
            float alphaAmount = 1;
            float fullAlphaTime = 1.5f;
            float fadeSpeed = .9f;

            yield return new WaitForSeconds(fullAlphaTime);

            while (alphaAmount > 0)
            {
                alphaAmount = alphaAmount - fadeSpeed * Time.deltaTime;
                Color textColor = new Color(exitsAreOpenMessageText.color.r, exitsAreOpenMessageText.color.g, exitsAreOpenMessageText.color.b, alphaAmount);
                exitsAreOpenMessageText.color = textColor;
                yield return null;
            }

            _onScreenLabelGOs.Remove(exitsAreOpenMessageGO);
            Destroy(exitsAreOpenMessageGO);
            yield return null;
        }

        public void ClearLabelsOnScreen()
        {
            StopAllCoroutines();
            for (int i = _onScreenLabelGOs.Count - 1; i >= 0; i--)
            {
                GameObject countdownTimerGO = _onScreenLabelGOs[i];
                _onScreenLabelGOs.Remove(_onScreenLabelGOs[i]);
                Destroy(countdownTimerGO);
            }
        }
    }
}
