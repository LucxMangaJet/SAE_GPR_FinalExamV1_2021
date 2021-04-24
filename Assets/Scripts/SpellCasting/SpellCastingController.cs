using System;
using System.Collections;
using UnityEngine;

public interface IPlayerAction
{
    bool IsInAction();
}

public class SpellCastingController : MonoBehaviour, IPlayerAction
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform castLocationTransform;
    [SerializeField] private ProjectileSpellDescription primarySpell;
    [SerializeField] private DropCollector dropCollector;


    private bool inAction;
    private float lastPrimarySpellTimestamp = -100;
    private float lastSecondarySpellTimestamp = -100;
    private SpellDescription secondarySpell;


    public SpellDescription PrimarySpell { get => primarySpell; }
    public SpellDescription SecondarySpell { get => secondarySpell; }

    public event System.Action<SpellSlot, SpellDescription> SpellCast;
    public event System.Action<SpellSlot, SpellDescription> EquippedSpellChanged;

    private void Start()
    {
        Debug.Assert(primarySpell, "No spell assigned to SpellCastingController.");
        Debug.Assert(dropCollector, "No drop collector referenced in SpellCastingController.");

        dropCollector.DropCollected += OnDropCollected;

    }

    private void OnDropCollected(Drop drop)
    {
        //picked up a spell drop
        if (drop is SpellDrop spellDrop)
        {
            if (spellDrop.SpellContained)
            {
                secondarySpell = spellDrop.SpellContained;
                EquippedSpellChanged?.Invoke(SpellSlot.Secondary, secondarySpell);
            }
        }
    }

    void Update()
    {
        bool simpleAttack = Input.GetButtonDown("Fire1");
        bool specialAttack = Input.GetButtonDown("Fire2");

        if (!inAction)
        {
            if (simpleAttack && GetPrimarySpellCooldown() == 0)
            {
                Cast(SpellSlot.Primary, primarySpell);
            }
            else if (specialAttack && secondarySpell && GetSecondarySpellCooldown() == 0)
            {
                Cast(SpellSlot.Secondary, secondarySpell);
            }
        }
    }

    private void Cast(SpellSlot slot, SpellDescription descr)
    {
        if(descr is ProjectileSpellDescription proj)
        {
            StartCoroutine(CastProjectileSpellRoutine(slot, proj));
        }
        else
        {
            //What happens when you cast a plane spell Description? The spell description includes no behaviour;
            Debug.LogError("SpellcastError: Cannot cast a plane SpellDescription");
        }
    }

    private IEnumerator CastProjectileSpellRoutine(SpellSlot slot, ProjectileSpellDescription spell)
    {
        SpellCast?.Invoke(slot, spell);
        inAction = true;
        animator.SetTrigger(spell.AnimationVariableName);

        yield return new WaitForSeconds(spell.ProjectileSpawnDelay);

        var projectile = Instantiate(spell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation);
        //inject damage
        if(projectile.TryGetComponent(out ISpellSpawnedBehaviour behavior))
        {
            behavior.SetDamage(spell.DamageDealt);
        }

        yield return new WaitForSeconds(spell.Duration - spell.ProjectileSpawnDelay);

        switch (slot)
        {
            case SpellSlot.Primary:
                lastPrimarySpellTimestamp = Time.time;
                break;
            case SpellSlot.Secondary:
                lastSecondarySpellTimestamp = Time.time;
                break;
        }

        inAction = false;
    }

    public bool IsInAction()
    {
        return inAction;
    }

    public float GetPrimarySpellCooldown()
    {
        return Mathf.Max(0, lastPrimarySpellTimestamp + primarySpell.Cooldown - Time.time);
    }

    public float GetSecondarySpellCooldown()
    {
        if (!secondarySpell) return 0;
        return Mathf.Max(0, lastSecondarySpellTimestamp + secondarySpell.Cooldown - Time.time);
    }
}
