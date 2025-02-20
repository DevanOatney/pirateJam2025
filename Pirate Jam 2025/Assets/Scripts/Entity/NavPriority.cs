public enum NavPriority
{
    // Targets the first enemy they spotted until they are no longer seen or dead
    First = 1,

    // Targets the closest enemy regardless
    Closest = 2,

    // Targets the enemy that most recently dealt damage to it
    Damage = 3
}
