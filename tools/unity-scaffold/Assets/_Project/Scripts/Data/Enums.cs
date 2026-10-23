namespace SLA.Data
{
    /// <summary>Hunter, gate and item rank, weakest to strongest.</summary>
    public enum Rank
    {
        E,
        D,
        C,
        B,
        A,
        S,
    }

    /// <summary>The five canon stats. Each one changes something the player can feel (GDD: Player progression).</summary>
    public enum StatType
    {
        Strength,
        Vitality,
        Agility,
        Perception,
        Intelligence,
    }

    /// <summary>Where a weapon hit landed; each zone has a damage multiplier in CombatConfig.</summary>
    public enum HitZone
    {
        Torso,
        Head,
        Limb,
        WeakPoint,
    }

    public enum WeaponType
    {
        Dagger,
        Sword,
        TwoHandedAxe,
    }

    public enum SkillActivation
    {
        Passive,
        GestureAndButton,
        ButtonOnly,
    }

    public enum QuestKind
    {
        Daily,
        Story,
        Emergency,
        Hidden,
    }

    public enum ItemKind
    {
        Weapon,
        Gloves,
        Armor,
        Consumable,
        Key,
        MonsterCore,
    }
}
