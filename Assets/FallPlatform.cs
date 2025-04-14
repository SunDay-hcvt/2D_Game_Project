using System.Collections;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    private float fallDelay = 0.5f;
    // private float destroyDelay = 1.5f;
    private float respawnDelay = 3f;
    [SerializeField] private Rigidbody2D rb2d;
    private Coroutine fallCoroutine;
    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            fallCoroutine = StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall() {
        yield return new WaitForSeconds(fallDelay);
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        //Destroy(gameObject, destroyDelay);
        yield return new WaitForSeconds(respawnDelay);
        ResetPlatform();
    }

    private void ResetPlatform() {
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.linearVelocity = Vector2.zero;
        rb2d.angularVelocity = 0f;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }

    public void CancelFall() {
        if (fallCoroutine != null) {
            StopCoroutine(fallCoroutine);
        }
    }
}
