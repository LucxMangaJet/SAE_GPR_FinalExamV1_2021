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
    [SerializeField] private ProjectileSpellDescription simpleAttackSpell;


    private bool inAction;
    private float lastSimpleAttackTimestamp = -100;

    public SpellDescription SimpleAttackSpellDescription { get => simpleAttackSpell; }

    public event System.Action<SpellSlot, SpellDescription> SpellCast;

    private void Start()
    {
        Debug.Assert(simpleAttackSpell, "No spell assigned to SpellCastingController.");
    }

    void Update()
    {
        bool simpleAttack = Input.GetButtonDown("Fire1");
        bool specialAttack = Input.GetButtonDown("Fire2");

        if (!inAction)
        {
            if (simpleAttack && GetSimpleAttackCooldown() == 0)
            {
                StartCoroutine(SimpleAttackRoutine());
            }
            else if (specialAttack)
            {
                Debug.Log("Trigger special attack");
            }
        }
    }

    private IEnumerator SimpleAttackRoutine()
    {
        SpellCast?.Invoke(SpellSlot.Primary, SimpleAttackSpellDescription);
        inAction = true;
        animator.SetTrigger(simpleAttackSpell.AnimationVariableName);

        yield return new WaitForSeconds(simpleAttackSpell.ProjectileSpawnDelay);

        Instantiate(simpleAttackSpell.ProjectilePrefab, castLocationTransform.position, castLocationTransform.rotation);

        yield return new WaitForSeconds(simpleAttackSpell.Duration - simpleAttackSpell.ProjectileSpawnDelay);

        lastSimpleAttackTimestamp = Time.time;
        inAction = false;
    }

    public bool IsInAction()
    {
        return inAction;
    }

    public float GetSimpleAttackCooldown()
    {
        return Mathf.Max(0, lastSimpleAttackTimestamp + simpleAttackSpell.Cooldown - Time.time);
    }
}
