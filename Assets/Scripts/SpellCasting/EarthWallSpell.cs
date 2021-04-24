using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthWallSpell : MonoBehaviour, ISpellSpawnedBehaviour
{
    private float damage = 0;
    [SerializeField] float wallStartY;
    [SerializeField] float wallEndY;
    [SerializeField] float growDuration;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] Rigidbody rigidbody;

    bool inGrowth;

    public void SetDamage(float descriptionDamage)
    {
        damage = descriptionDamage;
    }

    private void Start()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitRes, 5);

            //Check if on grass, otherwise cancel
        if (hit && hitRes.collider.TryGetComponent(out GrassMarker marker))
        {
            transform.Rotate(rotationOffset, Space.Self);
            StartCoroutine(GrowRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator GrowRoutine()
    {
        inGrowth = true;
        Vector3 startPosition = transform.position;
        Vector3 position = startPosition;
        for (float t = 0; t < growDuration;)
        {
            //move wall
            position.y = startPosition.y + Mathf.Lerp(wallStartY, wallEndY, t / growDuration);
            transform.position = position;

            yield return null;
            t += Time.deltaTime;
        }
        inGrowth = false;
        Destroy(rigidbody);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(inGrowth && collision.collider.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
        }
    }
}
