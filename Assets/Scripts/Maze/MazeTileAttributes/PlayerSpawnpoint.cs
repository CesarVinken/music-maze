public class PlayerSpawnpoint : CharacterSpawnpoint
{
    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        if (EditorManager.InEditor) return;

        MazeLevelManager.Instance.Level.PlayerCharacterSpawnpoints.Add(this);
    }
}
