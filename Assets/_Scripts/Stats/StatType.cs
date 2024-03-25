public enum StatType
{
    FireRate = 0,
    ReloadRate = 1,
    LifeTime = 2,
    BulletDamage = 3,
    SummonDamage = 4,
    SummonAttackSpeed = 5,
    Projectiles = 6,
    ProjectileSpeed = 7,
    ProjectileSize = 8,
    Knockback = 9,
    Red = 10,
    Yellow = 11,
    Blue = 12,
    Bounce = 13,
    Piercing = 14,
    Spread = 15,
    ManaRegen = 16,
    PickupRange = 17,
    VisionRange = 18,
    Dodge = 19,
    DodgeCapMod = 20,
    Count = 21
    //Innacuracy is not a StatType because maybe it's just simpler from a game perspective
    //that it's hardcoded and part of a spell's personality. Just better as a touch of vibe.
}

public enum YellowStatType
{
    NumProjectiles,
    Size,
    Lifetime
}