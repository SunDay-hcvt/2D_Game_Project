using UnityEngine;

public class Arrowtrap : MonoBehaviour
{
    [SerializeField] private float attackCooldonwn;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;
    private float cooldownTImer;

    [Header("SFX")]
    [SerializeField] private AudioClip arrowSound;

    private void Attack(){
        cooldownTImer = 0;
        SoundManager.instance.PlaySound(arrowSound);
        arrows[Findarrows()].transform.position = firePoint.position;
        arrows[Findarrows()].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private int Findarrows(){
        for (int i = 0; i< arrows.Length; i++){
            if(!arrows[i].activeInHierarchy){
                return i;
            }
        }
        return 0;
    }

    private void Update(){
        cooldownTImer += Time.deltaTime;

        if(cooldownTImer >= attackCooldonwn){
            Attack();
        }
    }
}
