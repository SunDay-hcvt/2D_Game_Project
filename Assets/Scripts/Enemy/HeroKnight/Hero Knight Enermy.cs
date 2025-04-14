using System;
using UnityEngine;

public class HeroKnightEnermy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float comboCooldown;

    [SerializeField] private float attack1_range;
    [SerializeField] private float attack1_coliderDistance;
    [SerializeField] private int attack1_Damage;

    [SerializeField] private float attack2_range;
    [SerializeField] private float attack2_coliderDistance;
    [SerializeField] private int attack2_Damage;

    [SerializeField] private BoxCollider2D boxCollider;
    [Header("Hero Knight Parameters")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask playerLayer;
    private bool isAttack1 = true;
    private float cooldownTimer = Mathf.Infinity;
    [SerializeField] private float idleDuration;
    private float idleTimer = 0;
    private bool movingLeft = true;

    [Header("Hero Knight Shield Parameters")]
    [SerializeField] private GameObject shield;
    [SerializeField] private int maxTakeHitCount;
    private int takeHitCount = 0;
    [SerializeField] private float shieldDuration;
    private float shieldDurationTimer = Mathf.Infinity;

    private float takeHitDuration = 2;
    private float takeHitTimer = Mathf.Infinity;

    [Header("Hero Knight Teleport Parameters")]
    [SerializeField] private GameObject teleport_object;
    [SerializeField] private float teleportCooldown;
    private float teleportTimer = Mathf.Infinity;
    [SerializeField] private float teleport_range;
    [SerializeField] private float teleport_coliderDistance;

    //Refferences
    private Animator anim;
    private Animator shieldAnim;
    private Health playerHealth;
    private PlayerMovement playerMovement;
    private HeroKnightEnermy heroKnightEnermy;
    private Health health;

    private float distanceToPlayer;
    private float minDistanceToChange;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        heroKnightEnermy = GetComponent<HeroKnightEnermy>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        shield.SetActive(false);
        shieldAnim = shield.GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.IsDead())
        {
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 4);
            return;
        }

        teleportTimer += Time.deltaTime;
        cooldownTimer += Time.deltaTime;

        if (IsTakeHit() || health.IsDead() || PlayerInSight_Attack1())
        {
            anim.SetBool("heroknight_moving", false);
        }
        else
        {
            if (PlayerInSight())
            {
                distanceToPlayer = playerMovement.transform.position.x - heroKnightEnermy.transform.position.x;
                minDistanceToChange = 0.1f;
                if (teleportTimer >= teleportCooldown)
                {
                    GameObject teleportskill = Instantiate(teleport_object, new Vector3(heroKnightEnermy.transform.position.x, heroKnightEnermy.transform.position.y, 2), Quaternion.identity);
                    teleportskill.transform.localScale = new Vector3(3, 5, 0);
                    Destroy(teleportskill, 1);
                    // anim.SetTrigger("heroknight_teleport");
                    TeleportInSight();
                    teleportskill = Instantiate(teleport_object, new Vector3(heroKnightEnermy.transform.position.x, heroKnightEnermy.transform.position.y, 2), Quaternion.identity);
                    teleportskill.transform.localScale = new Vector3(3, 5, 0);
                    Destroy(teleportskill, 1);
                }
                else
                {
                    if (movingLeft)
                    {
                        if (distanceToPlayer < -minDistanceToChange)
                        {
                            ChasePlayer(-1);
                        }
                        else
                        {
                            DirectionChange();
                        }
                    }
                    else
                    {
                        if (distanceToPlayer > minDistanceToChange)
                        {
                            ChasePlayer(1);
                        }
                        else
                        {
                            DirectionChange();
                        }
                    }
                }
            }
        }

        takeHitTimer += Time.deltaTime;
        shieldDurationTimer += Time.deltaTime;
        if (takeHitCount >= maxTakeHitCount)
        {
            shield.SetActive(true);
            shieldAnim.SetTrigger("active");
            shieldDurationTimer = 0;
            takeHitCount = 0;
        }
        if (shieldDurationTimer >= shieldDuration)
        {
            shieldAnim.SetTrigger("disable");
            shield.SetActive(false);
            shieldDurationTimer = 0;
            takeHitCount = 0;
        }

        //Attack only when player in sight?
        if (cooldownTimer <= attackCooldown && cooldownTimer >= comboCooldown)
        {
            if (isAttack1 == false && PlayerInSight_Attack2())
            {
                cooldownTimer = 0;
                anim.SetTrigger("heroknight_attack2");
                isAttack1 = true;
            }
        }
        else if (cooldownTimer >= attackCooldown)
        {
            if (isAttack1 == false)
            {
                isAttack1 = true;
            }
            if (PlayerInSight_Attack1())
            {
                cooldownTimer = 0;
                anim.SetTrigger("heroknight_attack1");
                isAttack1 = false;
            }
        }
    }

    private void TeleportInSight()
    {
        teleportTimer = 0;

        // Get the player's position and facing direction
        Vector3 playerPos = playerMovement.transform.position;
        float playerFacing = playerMovement.transform.localScale.x;

        // Calculate the position of the displacement behind the player
        float teleportOffset = 5f; // Adjust this distance if necessary.
        Vector3 teleportPos = playerPos;

        if (playerFacing > 0)
        {
            teleportPos.x -= teleportOffset; // Shift left
        }
        else
        {
            teleportPos.x += teleportOffset; // Shift right
        }

        //Make heroKnight face direction
        heroKnightEnermy.transform.localScale = new Vector3(
            heroKnightEnermy.transform.localScale.x,
            heroKnightEnermy.transform.localScale.y,
            heroKnightEnermy.transform.localScale.z
        );

        //Make in that direction
        heroKnightEnermy.transform.position = new Vector3(
            teleportPos.x,
            heroKnightEnermy.transform.position.y,
            heroKnightEnermy.transform.position.z
        );
    }


    private void DirectionChange()
    {
        anim.SetBool("heroknight_moving", false);

        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            idleTimer = 0;
            movingLeft = !movingLeft;
        }
    }

    private void ChasePlayer(int _direction)
    {
        anim.SetBool("heroknight_moving", true);

        // Di chuyển nhân vật về phía người chơi
        // Vector3 targetPos = new Vector3(playerPos.x, transform.position.y, transform.position.z);
        // transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        heroKnightEnermy.transform.position = new Vector3(
            heroKnightEnermy.transform.position.x + Time.deltaTime * _direction * moveSpeed,
            heroKnightEnermy.transform.position.y,
            heroKnightEnermy.transform.position.z);

        heroKnightEnermy.transform.localScale = new Vector3(
            Mathf.Abs(heroKnightEnermy.transform.localScale.x) * _direction,
            heroKnightEnermy.transform.localScale.y,
            heroKnightEnermy.transform.localScale.z
        );
    }

    private bool PlayerInSight_Attack1()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * attack1_range * transform.localScale.x * attack1_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * attack1_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z),
            0,
            Vector2.left,
            0,
            playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private bool PlayerInSight_Attack2()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * attack2_range * transform.localScale.x * attack2_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * attack2_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z),
            0,
            Vector2.left,
            0,
            playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center + transform.right * teleport_range * transform.localScale.x * teleport_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * teleport_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z),
            0,
            Vector2.left,
            0,
            playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.collider.GetComponent<Health>();
        }

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * attack1_range * transform.localScale.x * attack1_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * attack1_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * attack2_range * transform.localScale.x * attack2_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * attack2_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z));


        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            boxCollider.bounds.center + transform.right * teleport_range * transform.localScale.x * teleport_coliderDistance,
            new Vector3(
                boxCollider.bounds.size.x * teleport_range,
                boxCollider.bounds.size.y,
                boxCollider.bounds.size.z));
    }

    private void DamagePlayer_Attack1()
    {
        if (PlayerInSight_Attack1())
        {
            playerHealth.TakeDamage(attack1_Damage);
        }
    }

    private void DamagePlayer_Attack2()
    {
        if (PlayerInSight_Attack2())
        {
            playerHealth.TakeDamage(attack2_Damage);
        }
    }

    public void TakeHit()
    {
        takeHitCount++;
        takeHitTimer = 0;
    }

    public bool IsTakeHit()
    {
        return takeHitTimer <= takeHitDuration;
    }
}
