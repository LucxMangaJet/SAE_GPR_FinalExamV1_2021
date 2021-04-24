using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class InstantiatedSpellDescription : SpellDescription
{
    [Header("ToInstantiate")]
    [FormerlySerializedAs("ProjectilePrefab")]
    public GameObject PrefabToInstantiate;
    [FormerlySerializedAs("ProjectileSpawnDelay")]
    public float PrefabSpawnDelay;

    public float DamageDealt;
}
