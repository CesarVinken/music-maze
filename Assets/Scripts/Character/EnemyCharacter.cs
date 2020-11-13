using Photon.Pun;
using System;
using UnityEngine;

public class EnemyCharacter : Character
{
    public void Awake()
    {
        base.Awake();
        _characterPath.CharacterReachesTarget += OnTargetReached;

        CharacterManager.Instance.Enemies.Add(this);
    }

    public void Start()
    {
        GameManager.Instance.CompleteMazeLevelEvent += OnMazeLevelCompleted;
    }

    public void Update()
    {
        if (EditorManager.InEditor) return;

        if (IsFrozen) return;

        if (IsCalculatingPath) return;

        if (!HasCalculatedTarget &&
            (GameManager.Instance.GameType == GameType.SinglePlayer
            || PhotonView.IsMine))
        {
            SetNextTarget();
        }
        else
        {
            MoveCharacter();
        }
    }

    private void SetNextTarget()
    {
        IsCalculatingPath = true;
        int RandomMax = 20;
        int RandomNumber = UnityEngine.Random.Range(1, RandomMax + 1);
        float targetPlayerChance = 0.25f; // 25% chance to go chase a player

        if (RandomNumber <= targetPlayerChance * RandomMax) 
        {
            TargetPlayer();
        }
        else
        {
            SetRandomTarget();
        }
    }

    private void TargetPlayer()
    {
        //Randomly pick one of the players
        int randomNumber = UnityEngine.Random.Range(0, CharacterManager.Instance.MazePlayers.Count);

        PlayerCharacter randomPlayer = randomNumber == 0 ?
            CharacterManager.Instance.MazePlayers[PlayerNumber.Player1] :
            CharacterManager.Instance.MazePlayers[PlayerNumber.Player2];

        Vector3 playerVectorLocation = GridLocation.GridToVector(randomPlayer.CurrentGridLocation);

        // Known issue: The enemy will not plot a path to the actual location of the player, because the player's pathfinding node on the grid is already taken by the player. Thus the enemy will try to move next to the player 

        _seeker.StartPath(transform.position, playerVectorLocation, _characterPath.OnPathCalculated);

        Logger.Log(Logger.Pathfinding, $"The enemy {gameObject.name} is now going to the location of player {randomPlayer.gameObject.name} at {randomPlayer.CurrentGridLocation.X},{randomPlayer.CurrentGridLocation.Y}");
    }

    public void OnTargetReached()
    {
        //Logger.Log("enemy reached target");

        Vector3 roundedVectorPosition = new Vector3((float)Math.Round(transform.position.x - GridLocation.OffsetToTileMiddle), (float)Math.Round(transform.position.y - GridLocation.OffsetToTileMiddle));
        transform.position = new Vector3(roundedVectorPosition.x + GridLocation.OffsetToTileMiddle, roundedVectorPosition.y + GridLocation.OffsetToTileMiddle, 0);

        _animationHandler.SetLocomotion(false);
        SetHasCalculatedTarget(false);
    }

    public void OnMazeLevelCompleted()
    {
        //TODO: when maze level is completed, do not stop locomotion, but switch, if it exists, to an enemy idle animation
        FreezeCharacter();
    }
}
