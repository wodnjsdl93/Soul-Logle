using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentHealth;
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // 체력 감소 로그 출력
        Debug.Log("Player took damage: " + damage + ". Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 플레이어가 죽었을 때의 로직을 여기에 추가
        Debug.Log("Player died!");
    }
}
