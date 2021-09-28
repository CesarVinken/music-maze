using System;
using System.Collections.Generic;

namespace DataSerialisation
{
    [Serializable]
    public class SerialisableFerryRouteAttribute : ISerialisableTileAttribute
    {
        public string FerryRouteId;
        public List<SerialisableGridLocation> FerryRoutePoints;
        public int DockingStartDirection = -1;
        public int DockingEndDirection = -1;

        public SerialisableFerryRouteAttribute(string ferryRouteId, List<FerryRoutePoint> ferryRoutePoints, FerryDocking ferryDockingStart, FerryDocking ferryDockingEnd)
        {
            FerryRouteId = ferryRouteId;
            FerryRoutePoints = SerialiseFerryRoutePoints(ferryRoutePoints);
            
            DockingStartDirection = GetNumberFromDirection(ferryDockingStart.GetDockingDirection());
            if(FerryRoutePoints.Count > 1)
            {
                DockingEndDirection = GetNumberFromDirection(ferryDockingEnd.GetDockingDirection());
            }
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

        private int GetNumberFromDirection(Direction ferryDockingDirection)
        {
            switch (ferryDockingDirection)
            {
                case Direction.Right:
                    return 0;
                case Direction.Down:
                    return 1;
                case Direction.Left:
                    return 2;
                case Direction.Up:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}