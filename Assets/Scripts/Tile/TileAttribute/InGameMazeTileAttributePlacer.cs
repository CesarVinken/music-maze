using System.Collections.Generic;

public class InGameMazeTileAttributePlacer : MazeTileAttributePlacer<InGameMazeTile>
{
    private InGameMazeTile _tile;

    public override InGameMazeTile Tile { get => _tile; set => _tile = value; }

    public InGameMazeTileAttributePlacer(InGameMazeTile tile)
    {
        Tile = tile;
    }

    public void PlacePlayerSpawnpoint()
    {
        Logger.Log("TODO: only make not markable if there is a player for the spawnpoint");
        PlayerSpawnpoint playerSpawnpoint = (PlayerSpawnpoint)InstantiateTileAttributeGO<PlayerSpawnpoint>();

        Tile.SetWalkable(true);

        Tile.AddAttribute(playerSpawnpoint);
    }

    public override void PlaceFerryRoute(List<Tile> ferryRoutePointTiles, int dockingStartDirection, int dockingEndDirection)
    {
        FerryRoute ferryRoute = (FerryRoute)InstantiateTileAttributeGO<FerryRoute>();
        ferryRoute.SetTile(_tile);

        if (ferryRoutePointTiles == null)
        {
            ferryRoute.AddFerryRoutePointInGame(_tile);
        }
        else
        {
            for (int i = 0; i < ferryRoutePointTiles.Count; i++)
            {
                ferryRoute.AddFerryRoutePointInGame(ferryRoutePointTiles[i]);
            }
        }

        ferryRoute.Initialise(dockingStartDirection, dockingEndDirection);
        Tile.AddAttribute(ferryRoute);
    }
}