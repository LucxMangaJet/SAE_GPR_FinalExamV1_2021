using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricChainSpell : MonoBehaviour, ISpellSpawnedBehaviour
{
    [SerializeField] float initialMaxDistance;
    [SerializeField] float maxBounceRadius;
    [SerializeField] int maxBounceCount;
    [SerializeField] float distanceConsideredHit;
    [SerializeField] float speed;
    [SerializeField] Vector3 hitOffset;
    [SerializeField] GameObject particlesOnHit;

    AiTargetingManager targetingManager;
    Transform currentTarget = null;

    private float damage;

    void Start()
    {
        targetingManager = FindObjectOfType<AiTargetingManager>();

        if (!targetingManager)
        {
            Debug.LogError("ElectrigChainSpell failed, no targeting manager found");
            Destroy(gameObject);
            return;
        }

        currentTarget = GetInitialTarget();
        if (currentTarget)
        {
            StartCoroutine(BouncingRountine());
        }
        else
        {
            if (particlesOnHit)
                Instantiate(particlesOnHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private IEnumerator BouncingRountine()
    {
        //overarching bounce loop
        for (int bounce = 0; bounce < maxBounceCount; bounce++)
        {
            //move to target
            Vector3 offset = currentTarget.position + hitOffset - transform.position;
            float distance = offset.magnitude;
            while (distance > distanceConsideredHit)
            {
                transform.position += offset.normalized * speed * Time.deltaTime;
                yield return null;

                if (!currentTarget) break;
                offset = currentTarget.position + hitOffset - transform.position;
                distance = offset.magnitude;
            }

            //deal damage
            if (currentTarget && currentTarget.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(damage);
                if (particlesOnHit)
                    Instantiate(particlesOnHit, transform.position, Quaternion.identity);
            }

            //pick next target
            currentTarget = GetNextOptimalTarget();
            if (currentTarget == null) break;
        }

        Destroy(gameObject);

    }

    //   //closest target in radius that is in front of spell (half-sphere)
    public Transform GetInitialTarget()
    {
        List<IEnemy> enemies = targetingManager.GetAllEnemies();

        float minDist = float.MaxValue;
        Transform target = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            Transform enemy = enemies[i].transform;

            Vector3 dir = enemy.position - transform.position;

            //Skip enemies behind
            if (Vector3.Dot(transform.forward, dir.normalized) <= 0)
                continue;

            float distance = dir.magnitude;
            if (distance < minDist)
            {
                minDist = distance;
                target = enemy;
            }
        }

        if (minDist <= initialMaxDistance && target)
        {
            return target;
        }

        return null;
    }


    //closest target in radius (sphere)
    public Transform GetNextOptimalTarget()
    {
        List<IEnemy> enemies = targetingManager.GetAllEnemies();

        float minDist = float.MaxValue;
        Transform target = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            Transform enemy = enemies[i].transform;

            if (enemy == currentTarget) continue;

            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < minDist)
            {
                minDist = distance;
                target = enemy;
            }
        }

        if (minDist <= maxBounceRadius && target)
        {
            return target;
        }

        return null;
    }

    public void SetDamage(float descriptionDamage)
    {
        damage = descriptionDamage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, maxBounceRadius);
    }
}
