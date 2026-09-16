using UnityEngine;

public class InterestObject : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    public bool IsDestroyed => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDestroyed) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}