using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthWallSpell : MonoBehaviour, ISpellSpawnedBehaviour
{
    private float damage = 0;
    public void SetDamage(float descriptionDamage)
    {
        damage = descriptionDamage;
    }
}
