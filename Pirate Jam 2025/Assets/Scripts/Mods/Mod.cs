using System;

public class Mod
{
    public string name;
    public string description;

    public int powerPoints;
    public int defensePoints;
    public int speedPoints;

    public Action<Entity> onEquip;
}