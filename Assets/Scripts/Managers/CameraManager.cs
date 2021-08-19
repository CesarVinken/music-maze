using Character;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private GameObject _cameraPrefab;

    public float ScreenWidth = Screen.width;
    public float ScreenHeight = Screen.height;

    public List<CameraController> CameraControllers = new List<CameraController>();

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(_cameraPrefab, "_cameraPrefab", gameObject);

        if (Camera.main)
        {
            Destroy(Camera.main.gameObject);
        }

        if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
        {
            CreateTwoCameras();
        }
        else
        {
            CreateOneCamera();
        }
    }

    public void Start()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            //CameraControllers[i].SetZoomLevel(GameManager.Instance.Configuration.CameraZoomLevel);
            //CameraControllers[i].DisableCamera();
        }
    }

    public void ResetCameras()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            CameraControllers[i].ResetCamera();
        }
    }

    public void SetPanLimits()
    {
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            CameraControllers[i].SetPanLimits();
        }
    }

    public void CreateTwoCameras()
    {
        for (int i = CameraControllers.Count - 1; i >= 0; i--)
        {
            Destroy(CameraControllers[i].gameObject);
        }

        CameraControllers.Clear();

        GameObject splitScreenCameraOneGO = Instantiate(_cameraPrefab, transform);
        CameraController splitScreenCameraOne = splitScreenCameraOneGO.GetComponent<CameraController>();
        Camera splitScreenCameraOneCamera = splitScreenCameraOneGO.GetComponent<Camera>();
        splitScreenCameraOneCamera.rect = new Rect(0, 0, 0.5f, 1);
        splitScreenCameraOne.SetZoomLevel(GameManager.Instance.Configuration.DefaultCameraZoomLevel);

        GameObject splitScreenCameraTwoGO = Instantiate(_cameraPrefab, transform);
        CameraController splitScreenCameraTwo = splitScreenCameraTwoGO.GetComponent<CameraController>();
        Camera splitScreenCameraTwoCamera = splitScreenCameraTwoGO.GetComponent<Camera>();
        splitScreenCameraTwoCamera.rect = new Rect(0.5f, 0, 1, 1);
        splitScreenCameraTwo.SetZoomLevel(GameManager.Instance.Configuration.DefaultCameraZoomLevel);

        CameraControllers.Add(splitScreenCameraOne);
        CameraControllers.Add(splitScreenCameraTwo);

        ScreenWidth = Screen.width / 2;
        ScreenHeight = Screen.height / 2;
    }

    public void CreateOneCamera()
    {
        for (int i = CameraControllers.Count - 1; i >= 0; i--)
        {
            Destroy(CameraControllers[i].gameObject);
        }

        CameraControllers.Clear();

        GameObject mainCamera = Instantiate(_cameraPrefab, transform);
        CameraController soleCamera = mainCamera.GetComponent<CameraController>();
        Camera soleCameraCamera = soleCamera.GetComponent<Camera>();
        soleCameraCamera.rect.Set(0, 0, 1, 1);
        soleCamera.SetZoomLevel(GameManager.Instance.Configuration.DefaultCameraZoomLevel);

        CameraControllers.Add(soleCamera);

        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
    }

    public void FocusCamerasOnPlayer()
    {      
        for (int i = 0; i < CameraControllers.Count; i++)
        {
            PlayerCharacter player = null;

            if (PersistentGameManager.CurrentSceneType == SceneType.Maze)
            {
                if (GameRules.GamePlayerType == GamePlayerType.SinglePlayer)
                {
                    player = GameManager.Instance.CharacterManager.GetPlayerCharacter<MazePlayerCharacter>(PlayerNumber.Player1);
                }
                else if(GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
                {
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
                else if (GameRules.GamePlayerType == GamePlayerType.SplitScreenMultiplayer)
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
