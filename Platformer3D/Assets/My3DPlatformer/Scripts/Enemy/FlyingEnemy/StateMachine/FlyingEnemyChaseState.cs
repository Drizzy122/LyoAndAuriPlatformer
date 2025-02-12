using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyChaseState : FlyingEnemyBaseState
    {
        readonly Transform player;
        readonly float speed;
        readonly float stoppingDistance;
        readonly float rotationSpeed;

        public FlyingEnemyChaseState(FlyingEnemy flyingEnemy, Animator animator, Transform player, float speed, float stoppingDistance, float rotationSpeed)
            : base(flyingEnemy, animator)
        {
            this.player = player;
            this.speed = speed;
            this.stoppingDistance = stoppingDistance;
            this.rotationSpeed = rotationSpeed;
        }

        public override void OnEnter()
        {
           // Debug.Log("Chase");
            animator.CrossFade(FlyHash, crossFadeDuration);
        }

        public override void Update()
        {
            // Rotate toward the player
            RotateTowards(player.position);
            
            // Move toward player if not within stopping distance
            if (Vector3.Distance(flyingEnemy.transform.position, player.position) > stoppingDistance)
            {
                flyingEnemy.transform.position = Vector3.MoveTowards(flyingEnemy.transform.position, player.position, speed * Time.deltaTime);
            }
        }

        void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - flyingEnemy.transform.position).normalized; // Direction to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            flyingEnemy.transform.rotation = Quaternion.Slerp(flyingEnemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}