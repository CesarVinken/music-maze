using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private GameObject _mainCameraPrefab;
    [SerializeField] private GameObject _splitScreenCameraOnePrefab;
    [SerializeField] private GameObject _splitScreenCameraTwoPrefab;

    public CameraController MainCamera;
    public CameraController SplitScreenCameraOne;
    public CameraController SplitScreenCameraTwo;

    public List<CameraController> CameraControllers = new List<CameraController>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_mainCameraPrefab, "_mainCameraPrefab", gameObject);
        Guard.CheckIsNull(_splitScreenCameraOnePrefab, "_splitScreenCameraOnePrefab", gameObject);
        Guard.CheckIsNull(_splitScreenCameraTwoPrefab, "_splitScreenCameraTwoPrefab", gameObject);

        CameraControllers.Clear();

        if (GameRules.SplitScreen)
        {
            GameObject splitScreenCameraOneGO = Instantiate(_splitScreenCameraOnePrefab, transform);
            SplitScreenCameraOne = splitScreenCameraOneGO.GetComponent<CameraController>();
            GameObject splitScreenCameraTwoGO = Instantiate(_splitScreenCameraTwoPrefab, transform);
            SplitScreenCameraTwo = splitScreenCameraTwoGO.GetComponent<CameraController>();

            CameraControllers.Add(SplitScreenCameraOne);
            CameraControllers.Add(SplitScreenCameraTwo);
        }
        else
        {
            GameObject mainCamera = Instantiate(_mainCameraPrefab, transform);
            MainCamera = mainCamera.GetComponent<CameraController>();

            CameraControllers.Add(MainCamera);
        }
    }

    public void Start()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            CameraControllers[i].SetZoomLevel(GameManager.Instance.Configuration.CameraZoomLevel);
            CameraControllers[i].DisableCamera();
        }
    }

    public void ResetCameras()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            CameraControllers[i].ResetCamera();
        }
    }

    public void SetPanLimits(GridLocation levelBounds)
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            CameraControllers[i].SetPanLimits(levelBounds);
        }
    }

    public void FocusCamerasOnPlayer()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            PlayerCharacter player = null;

            if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                    player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);
                else
                {
                    player = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
                }
            }
            else
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                    player = GameManager.Instance.CharacterManager.GetPlayerCharacter<OverworldPlayerCharacter>(PlayerNumber.Player1);
                else
                {
                    player = GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
                }
            }

            if (player == null)
            {
                Logger.Error("Could not find player character on client");
            }
            Logger.Warning("Player is assigned only here!");

            CameraControllers[i].FocusOnPlayer(player);
        }

        for (int j = 0; j < CameraControllers.Count; j++)
        {
            CameraControllers[j].EnableCamera();
        }
    }
}
