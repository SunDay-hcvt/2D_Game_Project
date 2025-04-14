using System;
using UnityEngine;

public class HeroKnightPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [Header("Hero Knight")]
    [SerializeField] private Transform heroKnight;
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;
    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Hero Knight Animator")]
    [SerializeField] private Animator anim;

    private void Awake()
    {
        initScale = heroKnight.localScale;
    }

    private void OnDisable()
    {
        anim.SetBool("heroknight_moving", false);
    }

    private void Update()
    {
        if (heroKnight.GetComponent<HeroKnightEnermy>().IsTakeHit() || heroKnight.GetComponent<Health>().IsDead())
        {
            anim.SetBool("heroknight_moving", false);
        }
        else
        {
            if (movingLeft)
            {
                if (heroKnight.position.x >= leftEdge.position.x)
                {
                    MoveInDirection(-1);
                }
                else
                {
                    DirectionChange();
                }
            }
            else
            {
                if (heroKnight.position.x <= rightEdge.position.x)
                {
                    MoveInDirection(1);
                }
                else
                {
                    DirectionChange();
                }
            }
        }
    }

    private void DirectionChange()
    {
        anim.SetBool("heroknight_moving", false);

        idleTimer += Time.deltaTime;

        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
        }
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("heroknight_moving", true);

        //Make heroKnight face direction
        heroKnight.localScale = new Vector3(
            Mathf.Abs(initScale.x) * _direction,
            initScale.y,
            initScale.z);

        //Make in that direction
        heroKnight.position = new Vector3(
            heroKnight.position.x + Time.deltaTime * _direction * speed,
            heroKnight.position.y,
            heroKnight.position.z);
    }
}
