using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrapSpell : MonoBehaviour, ISpellSpawnedBehaviour
{
    [SerializeField] private GameObject hitEffectVisuals;
    [SerializeField] private GameObject runeVisuals;
    [SerializeField] private float timeBeforeDamage;
    [SerializeField] private float timeToDestroy;
    [SerializeField] private float radius;

    private bool triggered = false;
    private float damage = 0;

    void Start()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitRes, 5);
        if (hit)
        {
            transform.position = hitRes.point;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.TryGetComponent(out IEnemy damagable))
        {
            Debug.Log($"Fire trap triggered by {other.name}");
            Fire();
        }
    }

    private void Fire()
    {
        triggered = true;
        StartCoroutine(FireRoutine());
    }

    private IEnumerator FireRoutine()
    {
        hitEffectVisuals.SetActive(true);
        runeVisuals.SetActive(false);
        yield return new WaitForSeconds(timeBeforeDamage);

        //deal damage
        Debug.Log("Fire Trap Explosion damage triggered");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0.1f);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void SetDamage(float descriptionDamage)
    {
        damage = descriptionDamage;
    }
}
