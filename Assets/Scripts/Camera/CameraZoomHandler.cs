using System;
using UnityEngine;

public class CameraZoomHandler
{
    private float _zoomSpeed;
    private float _zoomMin;
    private float _zoomMax;

    private float _desiredZoomLevel;
    private float _zoomInCooldownTimeThreshold = 4f; // after how much time do we start zooming in back to the default level
    private float _zoomInCooldownTime = 4f; // 
    private float _velocity = 0f;

    private Camera _camera;

    public CameraZoomHandler(Camera camera)
    {
        _camera = camera;

        _zoomMin = GameManager.Instance.Configuration.DefaultCameraZoomLevel;
        _zoomMax = GameManager.Instance.Configuration.MaximumCameraZoomLevel;
        _zoomSpeed = GameManager.Instance.Configuration.DefaultZoomSpeed;

        _desiredZoomLevel = _camera.orthographicSize;
        Logger.Warning($"_desiredZoomLevel = {_desiredZoomLevel}");
    }
    public void HandleZoom()
    {
        float currentZoomLevel = _camera.orthographicSize;

        // Stop zooming if the difference with the desired zoom level is neglectible 
        if(Math.Abs(currentZoomLevel - _desiredZoomLevel) < 0.01f){
            if(currentZoomLevel < _zoomMin + 0.01f )
            {
                _desiredZoomLevel = _zoomMin;
                _camera.orthographicSize = _zoomMin;
                CameraController.CurrentZoom = ZoomAction.NoZoom;
            }
            else 
            {
                _desiredZoomLevel = currentZoomLevel;
                CameraController.CurrentZoom = ZoomAction.NoZoom;
            }
        }

        if(_desiredZoomLevel != _camera.orthographicSize)
        {
            ExecuteZooming(currentZoomLevel);
        }

        if(PersistentGameManager.CurrentPlatform == Platform.PC)
        {
            // Zoom out
            if(Input.GetKey(KeyCode.O) || Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                HandlePlayerZooming(ZoomType.ZoomOut);
            }
            else if(Input.GetKey(KeyCode.P) || Input.GetAxis("Mouse ScrollWheel") < 0f) // Zoom in
            {
                HandlePlayerZooming(ZoomType.ZoomIn);
            }
            else 
            {
                HandleAutoZoomInToBase();
            }
        }
        else // ANDROID
        {
            if (Input.touchCount == 2)
            {
                // Get current touch positions
                Touch touchOne = Input.GetTouch(0);
                Touch touchTwo = Input.GetTouch(1);

                // Get touch position from the previous frame
                Vector2 touchOnePrevious = touchOne.position - touchOne.deltaPosition;
                Vector2 touchTwoPrevious = touchTwo.position - touchTwo.deltaPosition;

                float oldTouchDistance = Vector2.Distance (touchOnePrevious, touchTwoPrevious);
                float currentTouchDistance = Vector2.Distance (touchOne.position, touchTwo.position);

                // Get movement offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;

                // Don't react to small movements
                if(deltaDistance <= 2.5f && deltaDistance >= -2.5f)
                {
                    return;
                }

                if(deltaDistance > 0)
                {
                    HandlePlayerZooming (ZoomType.ZoomIn);
                }
                else 
                {
                    HandlePlayerZooming (ZoomType.ZoomOut);
                }
            }
            else 
            {
                HandleAutoZoomInToBase();
            }
        }
    }

    private void ExecuteZooming(float currentZoomLevel)
    {
        // if we want to zoom out, check if there is space to zoom out further
        if (_desiredZoomLevel > _camera.orthographicSize)
        {
            Vector3 lowerLeftCornerView = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 upperLeftCornerView = _camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
            Vector3 lowerRightCornerView = _camera.ViewportToWorldPoint(new Vector3(1, 0, 0));

            //float halfScreenWidth = (lowerRightCornerView.x - lowerLeftCornerView.x) / 2;
            //float halfScreenHeight = (upperLeftCornerView.y - lowerLeftCornerView.y) / 2;
            float xPadding = 3f;
            float yPadding = 3f;
            float levelWidth = MazeLevelGameplayManager.Instance.Level.LevelBounds.X;
            float levelHeight = MazeLevelGameplayManager.Instance.Level.LevelBounds.Y;

            Logger.Log($"Camera transform x = {_camera.transform.position.x}. the left corner in view is: {lowerLeftCornerView.x}. Right corner in view is {lowerRightCornerView.x}. Right max is {levelWidth + xPadding}");

            if (lowerLeftCornerView.x <= -xPadding)
            {
                Logger.Warning($"Too far to the left. lowerLeftCornerView.x = {lowerLeftCornerView.x}. The limit is {-xPadding}");
                return;
            }
            if (lowerRightCornerView.x >= levelWidth + xPadding)
            {
                Logger.Warning($"Too far to the right. lowerRightCornerView.x = {lowerRightCornerView.x}. The limit is {levelWidth + xPadding}");
                return;
            }

            if (lowerLeftCornerView.y <= -yPadding)
            {
                Logger.Warning($"Too far to the bottom. lowerLeftCornerView.y = {lowerLeftCornerView.y}. The limit is {-yPadding}");
                return;
            }
            if (upperLeftCornerView.y >= levelWidth + yPadding)
            {
                Logger.Warning($"Too far to the top. upperLeftCornerView.y = {upperLeftCornerView.y}. The limit is {levelHeight + yPadding}");
                return;
            }

        }

        float smoothTime = 0.2f;
        float newZoomLevel = Mathf.SmoothDamp(currentZoomLevel, _desiredZoomLevel, ref _velocity, smoothTime, _zoomSpeed * 100 * Time.deltaTime);
        _camera.orthographicSize = Mathf.Clamp(newZoomLevel, _zoomMin, _zoomMax);
    }

    private void HandleAutoZoomInToBase()
    {
        if(_desiredZoomLevel != _zoomMin &&
            CameraController.CurrentZoom != ZoomAction.NoZoom){ // only when we are not touching AND there is no auto zoomin going on
            SetZoomingToNoZoom(); // start the process that will eventually zoom back to base level
        }

        if(_zoomInCooldownTime < _zoomInCooldownTimeThreshold)
        {
            _zoomInCooldownTime += 1f * Time.deltaTime;
        }
        else {
            if(_camera.orthographicSize > _zoomMin && _desiredZoomLevel != _zoomMin)
            {
                _desiredZoomLevel = _zoomMin;
                _zoomSpeed = 1f;
            }
        }
    }

    private void SetZoomingToNoZoom(){
        CameraController.CurrentZoom = ZoomAction.NoZoom;
        _zoomInCooldownTime = 0f;
        _velocity = 0f;
    }

    private void HandlePlayerZooming(ZoomType zoomType)
    {
        float currentZoomLevel = _camera.orthographicSize;

        if(zoomType == ZoomType.ZoomOut) // zoom out
        { 
            _desiredZoomLevel = _desiredZoomLevel - 1f;
        }
        else if(zoomType == ZoomType.ZoomIn) // zoom in
        { 
            _desiredZoomLevel = _desiredZoomLevel + 1f;
        }

        if(_desiredZoomLevel > currentZoomLevel + 1f){
            _desiredZoomLevel = currentZoomLevel + 1f;
        }
        else if(_desiredZoomLevel < currentZoomLevel - 1f){
           _desiredZoomLevel = currentZoomLevel - 1f;
        }

        if(_desiredZoomLevel < _zoomMin){
            _desiredZoomLevel = _zoomMin;
        }
        else if(_desiredZoomLevel > _zoomMax){
            _desiredZoomLevel = _zoomMax;
        }

        _zoomSpeed = GameManager.Instance.Configuration.DefaultZoomSpeed;
        CameraController.CurrentZoom = ZoomAction.PlayerZoom;
    }
}