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
        IEnumerator spawnExitsAreOpenMessageRoutine = SpawnExitsAreOpenMessage();
        StartCoroutine(spawnExitsAreOpenMessageRoutine);
    }

    public IEnumerator SpawnExitsAreOpenMessage()
    {
        GameObject exitsAreOpenMessageGO = Instantiate(_exitsAreOpenMessagePrefab, transform);
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
