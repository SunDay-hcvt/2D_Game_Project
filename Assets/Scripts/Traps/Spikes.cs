using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private Health playerHealth;

    [SerializeField] private float damage;
    [SerializeField] private float damageCooldown = 1f; // Thời gian giữa các lần gây sát thương

    private bool canDealDamage = true; // Kiểm soát việc gây sát thương

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            playerHealth = collision.GetComponent<Health>();
            StartCoroutine(ContinuousDamage());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            playerHealth = null;
            StopCoroutine(ContinuousDamage());
        }
    }

    private IEnumerator ContinuousDamage() {
        while (playerHealth != null) { // Gây sát thương liên tục nếu người chơi trong bẫy
            if (canDealDamage) {
                playerHealth.TakeDamage(damage);
                canDealDamage = false;
                yield return new WaitForSeconds(damageCooldown); // Chờ cooldown trước khi gây sát thương lần nữa
                canDealDamage = true;
            }
            yield return null;
        }
    }
}
