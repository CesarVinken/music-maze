public interface IPlatformConfiguration
{
    float DefaultCameraZoomLevel // 'Size'
    {
        get;
        set;
    }

    float CameraZoomSpeed
    {
        get;
        set;
    }

    float CameraAutoZoomSpeed
    {
        get;
        set;
    }

    float MaximumCameraZoomLevel
    {
        get;
        set;
    }
}

public class AndroidConfiguration : IPlatformConfiguration
{
    private float _defaultCameraZoomLevel;
    private float _maximumCameraZoomLevel;
    private float _cameraZoomSpeed;
    private float _cameraAutoZoomSpeed;

    public AndroidConfiguration()
    {
        DefaultCameraZoomLevel = 4f;
        MaximumCameraZoomLevel = 10f;
        CameraZoomSpeed = 8f;
        CameraAutoZoomSpeed = 0.30f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float CameraZoomSpeed { get { return _cameraZoomSpeed; } set { _cameraZoomSpeed = value; } }
    public float CameraAutoZoomSpeed { get { return _cameraAutoZoomSpeed; } set { _cameraAutoZoomSpeed = value; } }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _defaultCameraZoomLevel;
    private float _maximumCameraZoomLevel;
    private float _cameraZoomSpeed;
    private float _cameraAutoZoomSpeed;

    public PCConfiguration()
    {
        DefaultCameraZoomLevel = 7f;
        MaximumCameraZoomLevel = 12f;
        CameraZoomSpeed = 4f;
        CameraAutoZoomSpeed = 0.55f;
    }

    public float DefaultCameraZoomLevel { get { return _defaultCameraZoomLevel; } set { _defaultCameraZoomLevel = value; } }
    public float MaximumCameraZoomLevel { get { return _maximumCameraZoomLevel; } set { _maximumCameraZoomLevel = value; } }
    public float CameraZoomSpeed { get { return _cameraZoomSpeed; } set { _cameraZoomSpeed = value; } }
    public float CameraAutoZoomSpeed { get { return _cameraAutoZoomSpeed; } set { _cameraAutoZoomSpeed = value; } }
}