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
    private bool _isCooldownFromZoomToDefault = false;
    private Camera _camera;
    private float _velocity = 0f;
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
            float smoothTime = 0.2f;
            float newZoomLevel = Mathf.SmoothDamp(currentZoomLevel, _desiredZoomLevel, ref _velocity, smoothTime, _zoomSpeed * Time.deltaTime * 100);
            _camera.orthographicSize = Mathf.Clamp(newZoomLevel, _zoomMin, _zoomMax);
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
                Logger.Log($"deltaDistance {deltaDistance}");
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
                _zoomSpeed = 0.6f;
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