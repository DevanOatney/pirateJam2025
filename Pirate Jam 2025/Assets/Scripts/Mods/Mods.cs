using System.Collections.Generic;

public static class Mods
{
    // enemy can pull from this collection of mods to buff themselves
    public static HashSet<Mod> enemyMods = new HashSet<Mod>
    {
        new Mod
        {
            powerPoints = 1,
            onEquip = x => {}
        }
    };

    public static HashSet<Mod> mods = new HashSet<Mod>
    {
        new Mod
        {
            name = "Power Rune",
            description = "Increase damage by 10%.",
            powerPoints = 1,
            onEquip = x => { x.attackDamageMult += 0.1f; }
        },
        new Mod
        {
            name = "Defense Rune",
            description = "Increase max health by 10%.",
            defensePoints = 1,
            onEquip = x => { x.healthMult += 0.1f; }
        },
        new Mod
        {
            name = "Speed Rune",
            description = "Increase move speed by 10%.",
            powerPoints = 1,
            onEquip = x => { x.moveSpeedMult += 0.1f; }
        },

        // the combined mods as an example
        new Mod
        {
            name = "Haste Rune",
            description = "Increase attack speed by 10%.",
            powerPoints = 1,
            onEquip = x => { x.attackSpeedMult += 0.1f; }
        },
        new Mod
        {
            name = "Recovery Rune",
            description = "Increase health regen speed by 10%.",
            powerPoints = 1,
            onEquip = x => { x.healthRegenMult += 0.1f; }
        }
    };
}
