using UnityEngine;

[CreateAssetMenu]
public class ProjectileSpellDescription : SpellDescription
{
    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public float ProjectileSpawnDelay;

    public float DamageDealt;
}
