public enum ChasingState
{
    Active, // regular chasing behaviour, including occasional breaks
    Startled, // the enemy has starts around its head and will not catch any players it encounters. The enemy is still seeking random targets but not player targets
    Loitering // the enemy is not moving and not seeking targets
}