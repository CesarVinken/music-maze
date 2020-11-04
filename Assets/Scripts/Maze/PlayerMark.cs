public class PlayerMark
{
    public PlayerMarkOwner Owner { get; private set; }

    public PlayerMark()
    {
        Owner = PlayerMarkOwner.None;
    }

    public void SetOwner(PlayerMarkOwner owner)
    {
        Owner = owner;
    }
}