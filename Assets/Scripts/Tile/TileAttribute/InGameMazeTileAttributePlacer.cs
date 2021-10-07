using System.Collections.Generic;
using UnityEngine;

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

    public override void PlaceFerryRoute(string ferryRouteId, List<Tile> ferryRoutePointTiles, int ferryRouteDirection)
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

        ferryRoute.Initialise(ferryRouteId, ferryRouteDirection);
        Tile.AddAttribute(ferryRoute);

        for (int j = 0; j < ferryRoutePointTiles.Count; j++)
        {
            GameObject ferryRoutePointGO = GameObject.Instantiate(ferryRoute.FerryRoutePointSpritePrefab);
            ferryRoutePointGO.transform.SetParent(ferryRoutePointTiles[j].transform);
            ferryRoutePointGO.transform.position = ferryRoutePointGO.transform.parent.transform.position;

            FerryRoutePointSprite ferryRoutePointSprite = ferryRoutePointGO.GetComponent<FerryRoutePointSprite>();
            ferryRoutePointSprite.SetTile(ferryRoutePointTiles[j]);
            ferryRoutePointSprite.SetDirection(ferryRoute.FerryRouteDirection);
        }
    }
}