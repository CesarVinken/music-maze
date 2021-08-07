using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MazeLevelMainScreenOverlayCanvas : MonoBehaviour
{
    public static MazeLevelMainScreenOverlayCanvas Instance;

    [SerializeField] private GameObject _exitsAreOpenMessagePrefab;

    public void Awake()
    {
        Guard.CheckIsNull(_exitsAreOpenMessagePrefab, "_exitsAreOpenMessagePrefab", gameObject);

        Instance = this;
    }

    public void Start()
    {
        MazeLevelGameplayManager.Instance.AllPathsAreMarkedEvent += OnAllPathsAreMarked;
    }

    public void OnAllPathsAreMarked()
    {
        if(GameRules.GamePlayerType == GamePlayerType.SinglePlayer || GameRules.GamePlayerType == GamePlayerType.NetworkMultiplayer)
        {
            Vector3 spawnPosition = new Vector3(Screen.width / 2, Screen.height / 2);
            IEnumerator spawnExitsAreOpenMessageRoutine = SpawnExitsAreOpenMessage(spawnPosition);
            StartCoroutine(spawnExitsAreOpenMessageRoutine);
        }
        else // split screen requires two texts
        {
            Vector3 spawnPosition1 = new Vector3(Screen.width / 4, Screen.height / 2);
            Vector3 spawnPosition2 = new Vector3(Screen.width - Screen.width / 4, Screen.height / 2);

            IEnumerator spawnExitsAreOpenMessage1Routine = SpawnExitsAreOpenMessage(spawnPosition1);
            IEnumerator spawnExitsAreOpenMessage2Routine = SpawnExitsAreOpenMessage(spawnPosition2);

            StartCoroutine(spawnExitsAreOpenMessage1Routine);
            StartCoroutine(spawnExitsAreOpenMessage2Routine);
        }
    }

    public IEnumerator SpawnExitsAreOpenMessage(Vector3 spawnPosition)
    {
        GameObject exitsAreOpenMessageGO = Instantiate(_exitsAreOpenMessagePrefab, transform);
        exitsAreOpenMessageGO.transform.position = spawnPosition;

        Text exitsAreOpenMessageText = exitsAreOpenMessageGO.GetComponent<Text>();
        float alphaAmount = 1;
        float fullAlphaTime = 1.5f;
        float fadeSpeed = .9f;

        yield return new WaitForSeconds(fullAlphaTime);

        while (alphaAmount > 0)
        {
            alphaAmount = alphaAmount - (fadeSpeed * Time.deltaTime);
            Color textColor = new Color(exitsAreOpenMessageText.color.r, exitsAreOpenMessageText.color.g, exitsAreOpenMessageText.color.b, alphaAmount);
            exitsAreOpenMessageText.color = textColor;
            yield return null;
        }

        Destroy(exitsAreOpenMessageGO); 
        yield return null;
    }
}
