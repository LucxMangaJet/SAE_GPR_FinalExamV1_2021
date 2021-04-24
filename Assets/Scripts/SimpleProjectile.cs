using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float velocity;
    [SerializeField] private float damage;
    [SerializeField] private float selfdestructTime = 10;
    [SerializeField] private GameObject _effectOnHit;

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
}
