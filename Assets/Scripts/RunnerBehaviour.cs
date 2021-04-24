using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public interface IEnemy
{
    void InjectTargetingManager(AiTargetingManager inhectedTargetingManager);
    void InjectDropSpawner(DropSpawner injectedDropSpawner);

    public Transform transform { get; }
}


public class RunnerBehaviour : MonoBehaviour, IEnemy
{
    private static readonly int MOVEMENT_SPEED_ANIMATOR_ID = Animator.StringToHash("MovementSpeed");
    private static readonly int DEAD_ANIMATOR_ID = Animator.StringToHash("Dead");
    private static readonly int IS_HIT_ANIMATOR_ID = Animator.StringToHash("IsHit");

    [SerializeField] private Material[] skinMaterials;
    [SerializeField] private Renderer skinRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private LootDescription lootDescription;


    [SerializeField] private NavMeshAgent navMeshAgent;

    [FormerlySerializedAs("heathComponent")]
    [SerializeField] private HealthComponent healthComponent;

    [Header("Injected")]
    [SerializeField] private AiTargetingManager targetingManager;
    [SerializeField] private DropSpawner dropSpawner;

    private Transform target;

    private void Start()
    {
        if (skinMaterials != null && skinMaterials.Length > 0)
            skinRenderer.material = skinMaterials[Random.Range(0, skinMaterials.Length)];

        Debug.Assert(healthComponent != null, "HealthComponent not referenced in inspector");
        healthComponent.Death += OnDeath;
        healthComponent.Hit += OnHit;

        Debug.Assert(targetingManager != null, "TargetingManager not injected or referenced.");
        target = targetingManager.GetDefaultAITarget();
        navMeshAgent.SetDestination(target.position);
        targetingManager.RegisterEnemy(this);

    }
    private void OnDestroy()
    {
        if (targetingManager)
            targetingManager.RemoveEnemy(this);
    }

    private void Update()
    {
        animator.SetFloat(MOVEMENT_SPEED_ANIMATOR_ID, navMeshAgent.velocity.magnitude / navMeshAgent.speed);
    }
    private void OnHit(HealthComponent obj)
    {
        if (healthComponent.IsAlive)
            StartCoroutine(HitRoutine());
    }

    private void OnDeath(HealthComponent obj)
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        //Spawn loot
        if (lootDescription != null)
        {
            var drop = lootDescription.SelectDropRandomly();
            dropSpawner.SpawnDropAt(drop, transform.position);
        }

        animator.SetTrigger(DEAD_ANIMATOR_ID);
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }

    private IEnumerator HitRoutine()
    {
        animator.SetBool(IS_HIT_ANIMATOR_ID, true);
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(1);

        if (healthComponent.IsAlive)
        {
            animator.SetBool(IS_HIT_ANIMATOR_ID, false);
            navMeshAgent.isStopped = false;
        }
    }

    public void InjectTargetingManager(AiTargetingManager injectedTargetingManager)
    {
        targetingManager = injectedTargetingManager;

    }

    public void InjectDropSpawner(DropSpawner injectedDropSpawner)
    {
        dropSpawner = injectedDropSpawner;
    }
}
