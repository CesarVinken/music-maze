using System.Collections.Generic;
using UnityEngine;

public class EditorFerryRouteLineRenderer : MonoBehaviour
{
    private FerryRoute _ferryRoute;
    [SerializeField] private LineRenderer _lineRenderer;

    public void Initialise(FerryRoute ferryRoute)
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _ferryRoute = ferryRoute;
    }

    public void UpdateLineRenderer(List<FerryRoutePoint> ferryRoutePoints)
    { 
        if (ferryRoutePoints.Count < 2)
        {
            _lineRenderer.positionCount = 0;
        }
        else if (ferryRoutePoints.Count == 2)
        {
            _lineRenderer.positionCount = 2;
            for (int i = 0; i < ferryRoutePoints.Count; i++)
            {
                _lineRenderer.SetPosition(i, new Vector3(ferryRoutePoints[i].Tile.transform.position.x + 0.5f, ferryRoutePoints[i].Tile.transform.position.y + 0.5f, -1));
            }
        }
        else if (ferryRoutePoints.Count > _lineRenderer.positionCount) // in case we add a point
        {
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, new Vector3(ferryRoutePoints[ferryRoutePoints.Count - 1].Tile.transform.position.x + 0.5f, ferryRoutePoints[ferryRoutePoints.Count - 1].Tile.transform.position.y + 0.5f, -1));
        }
        else // in case we remove a point
        {
            _lineRenderer.positionCount = ferryRoutePoints.Count;
        }
    }
}
