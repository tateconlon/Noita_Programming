using UnityEngine;

public class ProjectileRecipe
{
    public string spawnID;
    public string objectPoolTag;

    public float damage;

    public float projectileSpeed;

    public float size;

    public float knockback;

    public int bounce;

    public int piercing;

    public float lifetime; //in other games this is just a component on the projectile

    public GameObject owner;
    
    public SpellItemV2 ownerSpell;
}