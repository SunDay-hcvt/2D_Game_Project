using UnityEngine;
using Pathfinding;

public class FlyingEyeEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

    [Header("Collider Parameters")]
    [SerializeField] private CircleCollider2D circleCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    // References
    private Animator anim;
    private Health playerHealth;
    private AIPath aiPath; // Thêm AIPath để xác định hướng di chuyển
    private Transform player; // Lưu vị trí của người chơi

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Xoay hướng theo chuyển động AIPath
        FlipTowardsMovementDirection();

        // Kiểm tra nếu người chơi trong phạm vi tấn công
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown && playerHealth.currentHealth > 0)
            {
                cooldownTimer = 0;
                anim.SetTrigger("flyingeye_attack");
                SoundManager.instance.PlaySound(attackSound);
            }
        }
    }

    // Luôn lật hướng theo AIPath để nhìn về hướng di chuyển
    private void FlipTowardsMovementDirection()
    {
        if (aiPath.desiredVelocity.x > 0) // Đi sang phải
        {
            transform.localScale = new Vector3(5, 5, 5); 
        }
        else if (aiPath.desiredVelocity.x < 0) // Đi sang trái
        {
            transform.localScale = new Vector3(-5, 5, 5);
        }
    }

    private bool PlayerInSight()
    {
        Collider2D hit = Physics2D.OverlapCircle(circleCollider.bounds.center, range, playerLayer);

        if (hit != null)
        {
            playerHealth = hit.GetComponent<Health>();
            player = hit.transform; // Lưu vị trí người chơi
        }

        return hit != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(circleCollider.bounds.center, range);
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }

}
