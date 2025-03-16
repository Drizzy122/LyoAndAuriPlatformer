using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Platformer {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    [RequireComponent(typeof(EnemyHealth))]
    public class Enemy : Entity {
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Self] PlayerDetector playerDetector;
        [SerializeField, Child] Animator animator;
        
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damageAmount = 10f;
        [SerializeField] public float rotationSpeed = 5f; // Adjust this in the Inspector
        
        [SerializeField] public float RunSpeed = 6f; // You can adjust this in the Inspector
        

        StateMachine stateMachine;
        private EnemyHealth enemyHealth;
        CountdownTimer attackTimer;

        void OnValidate() => this.ValidateRefs();

        void Start() {
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            stateMachine = new StateMachine();
            enemyHealth = GetComponent<EnemyHealth>();
            
            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            var dieState = new  EnemyDieState(this, animator); 
            
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));
            Any(dieState, new FuncPredicate(() => enemyHealth.currentHealth <= 0));
            
            stateMachine.SetState(wanderState);
            enemyHealth.OnDeath += () => stateMachine.SetState(dieState);
        }
        
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update() {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }
        
        void FixedUpdate() {
            stateMachine.FixedUpdate();
        }
        
        public void Attack() {
            if (attackTimer.IsRunning) return;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyAttack, this.transform.position);
            attackTimer.Start();

            // Access PlayerHealth and apply damage only if the player is not invulnerable
            var playerHealth = playerDetector.Player.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                if (!playerHealth.IsInvulnerable) { // Assuming you add a public IsInvulnerable property
                    playerHealth.TakeDamage(damageAmount);
                } else {
                    Debug.Log("Player is invulnerable. Attack did no damage.");
                }
            } else {
                Debug.LogWarning("PlayerHealth component not found on the player!");
            }
        }
    }
}