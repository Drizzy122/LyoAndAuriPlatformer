using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyWanderState : FlyingEnemyBaseState
    {
        readonly Vector3 startPoint;
        readonly float wanderRadius; 
        readonly float speed;
        readonly float rotationSpeed;
        
        Vector3 destination;
        
        public FlyingEnemyWanderState(FlyingEnemy flyingEnemy, Animator animator, float wanderRadius, float speed, float rotationSpeed) : base(flyingEnemy, animator)
        {
            this.startPoint = flyingEnemy.transform.position;
            this.wanderRadius = wanderRadius;
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.destination = GetRandomPoint();
        }
        
        public override void OnEnter() 
        {
            //Debug.Log("Wander");
            animator.CrossFade(FlyHash, crossFadeDuration);
        }

        public override void Update()
        {
            
            RotateTowards(destination);
            
            // Move towards destination
            flyingEnemy.transform.position = Vector3.MoveTowards(flyingEnemy.transform.position, destination, speed * Time.deltaTime);

            // If destination is reached, find a new random point
            if (Vector3.Distance(flyingEnemy.transform.position, destination) < 0.1f)
            {
                destination = GetRandomPoint();
            }
        }
        void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - flyingEnemy.transform.position).normalized; // Direction to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            flyingEnemy.transform.rotation = Quaternion.Slerp(flyingEnemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        Vector3 GetRandomPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            randomDirection.y = Mathf.Clamp(randomDirection.y, 1f, 10f); // Restrict height if needed
            return randomDirection;
            
        }
    }
}