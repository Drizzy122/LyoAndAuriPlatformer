using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyWanderState : EnemyBaseState
    {
        private readonly NavMeshAgent agent;
        private readonly Transform[] waypoints; // Array of waypoints
        private int currentWaypointIndex = 0; // Current waypoint
        
        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius,
            Transform[] waypoints) : base(enemy, animator)
        {
            this.agent = agent;
            this.waypoints = waypoints;
        }

        public override void OnEnter()
        {
            animator.CrossFade(WalkHash, crossFadeDuration);
            agent.speed = 2f; // Slow speed for wandering

            // Set initial waypoint destination
            if (waypoints.Length > 0)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                // Select next waypoint
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

                // Set destination to the next waypoint
                agent.SetDestination(waypoints[currentWaypointIndex].position);

                // Rotate smoothly towards the next waypoint
                Vector3 direction = (waypoints[currentWaypointIndex].position - enemy.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.rotationSpeed);
            }
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending
                   && agent.remainingDistance <= agent.stoppingDistance
                   && (!agent.hasPath || agent.velocity.sqrMagnitude <= Mathf.Epsilon);
        }
    }
}