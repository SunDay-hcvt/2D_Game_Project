using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float startingHealth;
    public float currentHealth { get; private set; }
    private Animator animator;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;


    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;


    [Header("Enemy HealthBar")]
    [SerializeField] private EnemyHealthBar enemyHealthBar; // Thêm biến thanh máu


    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();

        // Nếu đối tượng có thanh máu (chỉ enemy có)
        if (enemyHealthBar != null)
        {
            enemyHealthBar.SetMaxHealth((int)startingHealth);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // animator.SetTrigger("hurt");
            // animator.SetTrigger("wizard_hurt");
            // animator.SetTrigger("goblin_hurt");

            // Cập nhật thanh máu nếu có
            if (enemyHealthBar != null)
            {
                enemyHealthBar.SetHealth(currentHealth);
            }

            if (HasParameter(animator, "hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("hurt");
            }
            if (HasParameter(animator, "wizard_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("wizard_hurt");
            }
            if (HasParameter(animator, "goblin_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("goblin_hurt");
            }
            if (HasParameter(animator, "flyingeye_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("flyingeye_hurt");
            }
            if (HasParameter(animator, "skeleton_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("skeleton_hurt");
            }
            if (HasParameter(animator, "mushroom_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("mushroom_hurt");
            }
            if (HasParameter(animator, "cultistpriest_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("cultistpriest_hurt");
            }
            if (HasParameter(animator, "grimreaper_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("grimreaper_hurt");
            }
            if (HasParameter(animator, "fireworm_hurt", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("fireworm_hurt");
            }


            StartCoroutine(Invunerability());
            SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            if (!dead)
            {
                // Deactivate all attached component classes
                foreach (Behaviour component in components)
                {
                    component.enabled = false;
                }

                // animator.SetBool("grounded", true);
                // animator.SetTrigger("die");
                // animator.SetTrigger("wizard_died");
                // animator.SetTrigger("goblin_died");

                if (HasParameter(animator, "grounded", AnimatorControllerParameterType.Bool))
                {
                    animator.SetBool("grounded", true);
                }
                if (HasParameter(animator, "die", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("die");
                }
                if (HasParameter(animator, "wizard_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("wizard_died");
                }
                if (HasParameter(animator, "goblin_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("goblin_died");
                }
                if (HasParameter(animator, "flyingeye_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("flyingeye_died");
                }
                if (HasParameter(animator, "skeleton_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("skeleton_died");
                }
                if (HasParameter(animator, "mushroom_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("mushroom_died");
                }
                if (HasParameter(animator, "cultistpriest_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("cultistpriest_died");
                }
                if (HasParameter(animator, "grimreaper_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("grimreaper_died");
                }
                if (HasParameter(animator, "fireworm_died", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("fireworm_died");
                }


                dead = true;

                // Ẩn thanh máu khi enemy chết
                if (enemyHealthBar != null)
                {
                    enemyHealthBar.gameObject.SetActive(false);
                }
                SoundManager.instance.PlaySound(deathSound);
            }
        }
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        // animator.ResetTrigger("die");
        // animator.ResetTrigger("wizard_died");
        // animator.ResetTrigger("goblin_died");
        // animator.Play("Idle");
        // animator.Play("Wizard_Idle");
        // animator.Play("Goblin_Idle");


        if (HasParameter(animator, "die", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("die");
        }

        if (HasParameter(animator, "wizard_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("wizard_died");
        }

        if (HasParameter(animator, "goblin_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("goblin_died");
        }

        if (HasParameter(animator, "flyingeye_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("flyingeye_died");
        }

        if (HasParameter(animator, "skeleton_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("skeleton_died");
        }

        if (HasParameter(animator, "mushroom_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("mushroom_died");
        }

        if (HasParameter(animator, "cultistpriest_died", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("cultistpriest_died");
        }

        if (HasParameter(animator, "grimreaper_died", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger("grimreaper_died");
        }
        if (HasParameter(animator, "fireworm_died", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger("fireworm_died");
        }




        if (animator.HasState(0, Animator.StringToHash("Idle")))
        {
            animator.Play("Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("Wizard_Idle")))
        {
            animator.Play("Wizard_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("Goblin_Idle")))
        {
            animator.Play("Goblin_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("Flyingeye_Flight")))
        {
            animator.Play("Flyingeye_Flight");
        }

        if (animator.HasState(0, Animator.StringToHash("Skeleton_Idle")))
        {
            animator.Play("Skeleton_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("Mushroom_Idle")))
        {
            animator.Play("Mushroom_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("CultistPriest_Idle")))
        {
            animator.Play("CultistPriest_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("GrimReaper_Idle")))
        {
            animator.Play("GrimReaper_Idle");
        }

        if (animator.HasState(0, Animator.StringToHash("FireWorm_Idle")))
        {
            animator.Play("FireWorm_Idle");
        }




        StartCoroutine(Invunerability());
        // Activate all attached component classes
        foreach (Behaviour component in components)
        {
            component.enabled = true;
        }
    }

    private IEnumerator Invunerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == type)
            {
                return true;
            }
        }
        return false;
    }

}
