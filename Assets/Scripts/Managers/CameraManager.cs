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

    public float ScreenWidth = Screen.width;
    public float ScreenHeight = Screen.height;

    public List<CameraController> CameraControllers = new List<CameraController>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_mainCameraPrefab, "_mainCameraPrefab", gameObject);
        Guard.CheckIsNull(_splitScreenCameraOnePrefab, "_splitScreenCameraOnePrefab", gameObject);
        Guard.CheckIsNull(_splitScreenCameraTwoPrefab, "_splitScreenCameraTwoPrefab", gameObject);

        CameraControllers.Clear();

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
        {
            GameObject splitScreenCameraOneGO = Instantiate(_splitScreenCameraOnePrefab, transform);
            SplitScreenCameraOne = splitScreenCameraOneGO.GetComponent<CameraController>();
            GameObject splitScreenCameraTwoGO = Instantiate(_splitScreenCameraTwoPrefab, transform);
            SplitScreenCameraTwo = splitScreenCameraTwoGO.GetComponent<CameraController>();

            CameraControllers.Add(SplitScreenCameraOne);
            CameraControllers.Add(SplitScreenCameraTwo);

            ScreenWidth = Screen.width / 2;
            ScreenHeight = Screen.height / 2;
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
        if (Camera.main)
        {
            Destroy(Camera.main.gameObject);
        }
        
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            PlayerCharacter player = null;

            if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                {
                    player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);
                }
                else if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
                {
                    Logger.Warning("TODO FOR SPLIT SCREEN");
                    if (i == 0)
                    {
                        player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);
                    }
                    else if (i == 1)
                    {
                        player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player2);
                    }
                    else
                    {
                        Logger.Error("There seem to be too many cameras registered.");
                    }
                }
                else
                {
                    player = GameManager.Instance.CharacterManager.GetPlayers<MazePlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
                }
            }
            else
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                {
                    player = GameManager.Instance.CharacterManager.GetPlayerCharacter<OverworldPlayerCharacter>(PlayerNumber.Player1);
                }
                else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiPlayer)
                {
                    if(i == 0)
                    {
                        player = GameManager.Instance.CharacterManager.GetPlayerCharacter<OverworldPlayerCharacter>(PlayerNumber.Player1);
                    } 
                    else if (i == 1)
                    {
                        player = GameManager.Instance.CharacterManager.GetPlayerCharacter<OverworldPlayerCharacter>(PlayerNumber.Player2);
                    }
                    else
                    {
                        Logger.Error("There seem to be too many cameras registered.");
                    }
                }
                else
                {
                    player = GameManager.Instance.CharacterManager.GetPlayers<OverworldPlayerCharacter>().FirstOrDefault(p => p.Value.PhotonView.IsMine).Value;
                }
            }

            if (player == null)
            {
                Logger.Error("Could not find player character on client");
            }
            //Logger.Warning("Player is assigned only here!");

            CameraControllers[i].FocusOnPlayer(player);
        }

        for (int j = 0; j < CameraControllers.Count; j++)
        {
            CameraControllers[j].EnableCamera();
        }
    }
}
