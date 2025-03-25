using UnityEngine;

namespace Platformer
{
    public class FlyingEnemyWanderState : FlyingEnemyBaseState
    {
        private readonly Transform[] waypoints;
        private readonly float speed;
        private readonly float rotationSpeed;

        private int currentWaypointIndex;

        public FlyingEnemyWanderState(FlyingEnemy flyingEnemy, Animator animator, Transform[] waypoints, float speed, float rotationSpeed)
            : base(flyingEnemy, animator)
        {
            this.waypoints = waypoints;
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            this.currentWaypointIndex = 0; // Start at the first waypoint
        }

        public override void OnEnter() 
        {
            animator.CrossFade(FlyHash, crossFadeDuration);
        }

        public override void Update()
        {
            if (waypoints.Length == 0) return; // No waypoints to follow

            // Get the current waypoint
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            // Rotate towards the current waypoint
            RotateTowards(targetPosition);
            
            // Move towards the current waypoint
            flyingEnemy.transform.position = Vector3.MoveTowards(
                flyingEnemy.transform.position, 
                targetPosition, 
                speed * Time.deltaTime
            );

            // Check if the enemy is close enough to the waypoint
            if (Vector3.Distance(flyingEnemy.transform.position, targetPosition) < 0.1f)
            {
                // Move to the next waypoint, looping back to the start if at the end
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }

        private void RotateTowards(Vector3 target)
        {
            Vector3 direction = (target - flyingEnemy.transform.position).normalized; // Direction to face
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            flyingEnemy.transform.rotation = Quaternion.Slerp(
                flyingEnemy.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
}