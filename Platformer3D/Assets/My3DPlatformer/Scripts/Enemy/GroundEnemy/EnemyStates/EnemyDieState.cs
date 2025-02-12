using UnityEngine;

namespace Platformer
{
    public class EnemyDieState : EnemyBaseState
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public EnemyDieState(Enemy enemy, Animator animator) : base(enemy, animator)
        {
            //Noop   
        }

        public override void OnEnter()
        {
            Debug.Log("Enemy Is Dead");
            animator.CrossFade(DieHash, crossFadeDuration);
            
            // Disable Enemy Colliders or any other relevant components
            enemy.GetComponent<Collider>().enabled = false;
            GameObject.Destroy(enemy.gameObject, 3f);
        }

        public override void Update()
        {
            //Noop
        }
    }
}
