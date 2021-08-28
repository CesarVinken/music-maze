namespace Character
{
    public enum ChasingState
    {
        Active, // regular chasing behaviour, including occasional breaks
        Startled, // the enemy has starts around its head and will not catch any players it encounters. The enemy is still seeking random targets but not player targets
        Loitering, // the enemy is not moving and not seeking targets
        Reading // Like loitering, the enemy is not moving and not seeking targets. Separated from Loitering to avoid incorrect triggers that would reset it to active state
    }
}
