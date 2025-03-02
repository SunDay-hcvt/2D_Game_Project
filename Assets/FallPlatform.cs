using System.Collections;
using UnityEngine;

public class FallPlatform : MonoBehaviour
{
    private float fallDelay = 0.5f;
    private float destroyDelay = 1.5f;
    [SerializeField] private Rigidbody2D rb2d;
    private Coroutine fallCoroutine;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            fallCoroutine = StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall() {
        yield return new WaitForSeconds(fallDelay);
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, destroyDelay);
    }

    public void CancelFall() {
        if (fallCoroutine != null) {
            StopCoroutine(fallCoroutine);
        }
    }
}
