public interface IPlatformConfiguration
{
    float DefaultCameraZoomLevel // 'Size'
    {
        get;
        set;
    }

    float DefaultZoomSpeed
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
    private float _defaultZoomSpeed;
  //private float _panSpeed;

    public AndroidConfiguration()
    {
        DefaultCameraZoomLevel = 4f;
        MaximumCameraZoomLevel = 10f;
        DefaultZoomSpeed = 8f;
        //    PanSpeed = 1.3f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float DefaultZoomSpeed { get { return _defaultZoomSpeed; } set { _defaultZoomSpeed = value; } }
    //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _defaultCameraZoomLevel;
    private float _maximumCameraZoomLevel;
    private float _defaultZoomSpeed;
    //private float _panSpeed;

    public PCConfiguration()
    {
        DefaultCameraZoomLevel = 7f;
        MaximumCameraZoomLevel = 12f;
        DefaultZoomSpeed = 4f;
        //    PanSpeed = 10f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float DefaultZoomSpeed { get { return _defaultZoomSpeed; } set { _defaultZoomSpeed = value; } }
  //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
}