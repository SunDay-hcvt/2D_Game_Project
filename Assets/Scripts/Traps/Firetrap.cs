using UnityEngine;
using System.Collections;

public class Firetrap : MonoBehaviour
{
    [SerializeField] private float damage;
    [Header("Firetrap Timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool triggered; //when the traps get triggered
    private bool active; // when the trap is active and can hurt the player

    private Health playerHealth;

    [Header("SFX")]
    [SerializeField] private AudioClip firetrapSound;

    private void Awake(){
        anim = GetComponent<Animator>();
        spriteRend  = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if(playerHealth != null && active) {
            playerHealth.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player"){
            playerHealth = collision.GetComponent<Health>();

            if(!triggered){
                // trigger the firetrap
                StartCoroutine(ActivateFiretrap());
            }
            if(active){
                collision.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player"){
            playerHealth = null;
        }
    }

    private IEnumerator ActivateFiretrap(){
        // tủn the sprite red to notify player
        triggered = true;
        spriteRend.color = Color.red; 

        // Wait for delaye, active trap, turn on animation, return color back to normal
        yield return new WaitForSeconds(activationDelay);
        SoundManager.instance.PlaySound(firetrapSound);
        spriteRend.color = Color.white; // turn the sprite back to its initial color
        active = true;
        anim.SetBool("activated", true);

        // Wait until X seconds, deactivate trap and reset all variables and animator
        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("activated", false);
    }
}
