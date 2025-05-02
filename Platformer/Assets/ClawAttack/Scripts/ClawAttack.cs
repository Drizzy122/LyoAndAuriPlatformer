using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class ClawAttack : MonoBehaviour
{
    [SerializeField] float cooldown = 2f;
    [SerializeField] Animator animatorController;
    [SerializeField] GameObject clawAttackPrefab;
    [SerializeField] Transform firePoint;

    bool attacking;
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!attacking)
            {
                attacking = true;
                animatorController.SetBool("attack", attacking);
            }            
        }
    }

    IEnumerator ClawAttackCo() //called in animation
    {
        if(animatorController != null)
        {
            if(clawAttackPrefab != null && firePoint != null)
            {
                GameObject clawAttack = Instantiate(clawAttackPrefab, firePoint.localPosition, clawAttackPrefab.transform.rotation) as GameObject;
                Destroy(clawAttack, 3f);
            }
            else
            {
                Debug.Log("Claw Attack Prefab or Fire Point not assigned.");
            }
        }
        else
        {
            Debug.Log ("Animator Controller not assigned.");
        }

        yield return new WaitForSeconds(cooldown);

        attacking = false;
        animatorController.SetBool("attack", attacking);
    }
}
