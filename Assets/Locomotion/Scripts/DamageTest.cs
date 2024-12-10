using UnityEngine;

public class DamageTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            FirstPersonController.OnTakeDamage(15);
            damageable.ApplyDamage(1);
        }
    }
}
