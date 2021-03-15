
public abstract class TileBackgroundRemover
{
    public abstract void RemovePath();
    public abstract void RemoveBackground<T>() where T : ITileBackground;
}

