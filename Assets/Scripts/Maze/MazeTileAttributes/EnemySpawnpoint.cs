public class EnemySpawnpoint : CharacterSpawnpoint
{
    public override void RegisterSpawnpoint()
    {
        GridLocation = GridLocation.VectorToGrid(transform.position);

        if (EditorManager.InEditor) return;

        MazeLevelManager.Instance.Level.EnemyCharacterSpawnpoints.Add(this);
    }
}
