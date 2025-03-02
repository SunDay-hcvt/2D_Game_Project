using Pathfinding;
using UnityEngine;

public class CultistPriestEnemy_1 : MonoBehaviour
{

    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

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
    private AIPath aiPath; // Thêm AIPath để xác định hướng di chuyển
    private Transform player; // Lưu vị trí của người chơi

    private void Awake()
    {
        anim = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Xoay hướng theo chuyển động AIPath
        FlipTowardsMovementDirection();

        if(PlayerInChaseRange() && !PlayerInSight()){
            anim.SetBool("cultistpriest_moving", true);
        }

        // Kiểm tra nếu người chơi trong phạm vi tấn công
        if (PlayerInSight())
        {
            aiPath.canMove = false;
            anim.SetBool("cultistpriest_moving", false);
            if (cooldownTimer >= attackCooldown && playerHealth.currentHealth > 0)
            {
                cooldownTimer = 0;
                anim.SetTrigger("cultistpriest_attack");
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
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            return playerHealth != null && playerHealth.currentHealth > 0;
        }
        return false;
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
            playerHealth.TakeDamage(damage);
        }
    }

    private bool PlayerInChaseRange()
    {
        return aiPath.hasPath; // Nếu có đường đến Player, tiếp tục đuổi
    }

}
