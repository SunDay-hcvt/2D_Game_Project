using UnityEngine;

public class GrimReaperEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    // References
    private Animator anim;
    private Health playerHealth;

    private GrimReaperPatrol grimReaperPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        grimReaperPatrol = GetComponentInParent<GrimReaperPatrol>();

        foreach (var param in anim.parameters)
        {
            Debug.Log("Animator Parameter: " + param.name);
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // attack only when player in sight
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown && playerHealth.currentHealth > 0)
            {
                // attack
                cooldownTimer = 0;
                anim.SetTrigger("grimreaper_attack");
                SoundManager.instance.PlaySound(attackSound);
                Debug.Log("Player detected, attacking...");
                Debug.Log("player in sight");
            }
        }

        if (grimReaperPatrol != null)
        {
            grimReaperPatrol.enabled = !PlayerInSight();
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
        }
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            // Damage player health
            playerHealth.TakeDamage(damage);
        }
    }
}
