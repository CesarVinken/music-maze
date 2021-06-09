public interface IPlatformConfiguration
{
    float CameraZoomLevel // 'Size'
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
    private float _cameraZoomLevel;
    //private float _panSpeed;
    //private float _zoomModifierSpeed;

    public AndroidConfiguration()
    {
        CameraZoomLevel = 4;
        //    PanSpeed = 1.3f;
        //    ZoomModifierSpeed = 0.032f;
    }

    public float CameraZoomLevel { get { return _cameraZoomLevel; } set { _cameraZoomLevel = value; } }
    //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    //public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _cameraZoomLevel;
    //private float _panSpeed;
    //private float _zoomModifierSpeed;

    public PCConfiguration()
    {
        CameraZoomLevel = 7;
        //    PanSpeed = 10f;
        //    ZoomModifierSpeed = 14f;
    }

    public float CameraZoomLevel { get { return _cameraZoomLevel; } set { _cameraZoomLevel = value; } }
    //public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    //public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}