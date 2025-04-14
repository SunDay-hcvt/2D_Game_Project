using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;
    private bool hit;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }
    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision){
    //     hit = true;
    //     base.OnTriggerEnter2D(collision); // Execute logic from parent script first
    //     coll.enabled = false;

    //     if(anim != null){
    //         anim.SetTrigger("explode"); //when object is fireball explode it
    //     } else {
    //         gameObject.SetActive(false);// when this hits any object deactivate row
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        base.OnTriggerEnter2D(collision); // Thực hiện logic từ lớp cha trước
        coll.enabled = false;

        // In thông tin vật thể va chạm
        // Debug.Log("Fireball va chạm với: " + collision.gameObject.name);
        // Debug.Log("Tag của vật thể: " + collision.gameObject.tag);
        // Debug.Log("Layer của vật thể: " + LayerMask.LayerToName(collision.gameObject.layer));

        if (anim != null)
        {
            anim.SetTrigger("explode"); // Kích hoạt hiệu ứng nổ
        }
        else
        {
            gameObject.SetActive(false); // Nếu không có animation thì ẩn đối tượng
        }
    }


    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
