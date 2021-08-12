public interface IPlatformConfiguration
{
    float DefaultCameraZoomLevel // 'Size'
    {
        get;
        set;
    }

    float ZoomSpeed
    {
        get;
        set;
    }

    float MaximumCameraZoomLevel
    {
        get;
        set;
    }
    //float PanSpeed
    //{
    //    get;
    //    set;
    //}
    //float ZoomModifierSpeed
    //{
    //    get;
    //    set;
    //}
}

public class AndroidConfiguration : IPlatformConfiguration
{
    private float _defaultCameraZoomLevel;
    private float _maximumCameraZoomLevel;
    private float _zoomSpeed;
  //private float _panSpeed;
    //private float _zoomModifierSpeed;

    public AndroidConfiguration()
    {
        DefaultCameraZoomLevel = 4f;
        MaximumCameraZoomLevel = 10f;
        ZoomSpeed = 4f;
        //    PanSpeed = 1.3f;
        //    ZoomModifierSpeed = 0.032f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float ZoomSpeed { get { return _zoomSpeed; } set { _zoomSpeed = value; } }
    //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    //public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _defaultCameraZoomLevel;
    private float _maximumCameraZoomLevel;
    private float _zoomSpeed;
    //private float _panSpeed;
    //private float _zoomModifierSpeed;

    public PCConfiguration()
    {
        DefaultCameraZoomLevel = 7f;
        MaximumCameraZoomLevel = 12f;
        ZoomSpeed = 4f;
        //    PanSpeed = 10f;
        //    ZoomModifierSpeed = 14f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float ZoomSpeed { get { return _zoomSpeed; } set { _zoomSpeed = value; } }
  //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    //public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}