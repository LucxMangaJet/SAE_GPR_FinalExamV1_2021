using UnityEngine;


public interface ISpellSpawnedBehaviour
{
    public void SetDamage(float descriptionDamage);
}

public class SimpleProjectile : MonoBehaviour, ISpellSpawnedBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float velocity;
    [SerializeField] private float selfdestructTime = 10;
    [SerializeField] private GameObject _effectOnHit;

    private float damage = 0;
    private void Start()
    {
        _rigidbody.velocity = transform.forward * velocity;
        Destroy(gameObject, selfdestructTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);

            if (_effectOnHit)
                Instantiate(_effectOnHit, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void SetDamage(float descriptionDamage)
    {
        damage = descriptionDamage;
    }
}
