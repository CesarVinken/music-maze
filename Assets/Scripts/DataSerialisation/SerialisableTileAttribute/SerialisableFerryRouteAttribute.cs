using System;
using System.Collections.Generic;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableFerryRouteAttribute : ISerialisableTileAttribute
    {
        public List<SerialisableGridLocation> FerryRoutePoints;

        public SerialisableFerryRouteAttribute(List<FerryRoutePoint> ferryRoutePoints)
        {
            FerryRoutePoints = SerialiseFerryRoutePoints(ferryRoutePoints);
        }

        private List<SerialisableGridLocation> SerialiseFerryRoutePoints(List<FerryRoutePoint> ferryRoutePoints)
        {
            List<SerialisableGridLocation> ferryRoutePointGridLocations = new List<SerialisableGridLocation>();

            for (int i = 0; i < ferryRoutePoints.Count; i++)
            {
                Tile routePointTile = ferryRoutePoints[i].Tile;
                ferryRoutePointGridLocations.Add(new SerialisableGridLocation(routePointTile.GridLocation.X, routePointTile.GridLocation.Y));
            }

            return ferryRoutePointGridLocations;
        }
    }
}