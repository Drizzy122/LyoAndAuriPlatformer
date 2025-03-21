using UnityEngine;
using KBCore.Refs;
using Utilities;

namespace Platformer {
    [RequireComponent(typeof(PlayerDetector))]
     [RequireComponent(typeof(EnemyHealth))]
    
    public class FlyingEnemy : Entity
    {
        [SerializeField, Self] PlayerDetector playerDetector;
        [SerializeField, Child] Animator animator;
        [SerializeField] float speed = 5f; // Flying speed
        [SerializeField] float stoppingDistance = 1f;

        [SerializeField] float wanderRadius = 10f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damageAmount = 10f;
        
        private EnemyHealth enemyHealth;
        StateMachine stateMachine;
        CountdownTimer attackTimer;
        
        void OnValidate() => this.ValidateRefs();
        
        void Start()
        {
            float rotationSpeed = 3f; // Adjust rotation speed here
           

            
            enemyHealth = GetComponent<EnemyHealth>();
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            stateMachine = new StateMachine();
            
            
            var wanderState = new FlyingEnemyWanderState(this, animator, wanderRadius, speed, rotationSpeed);
            var chaseState = new FlyingEnemyChaseState(this, animator, playerDetector.Player, speed, stoppingDistance, rotationSpeed);
            var attackState = new FlyingEnemyAttackState(this, animator, playerDetector.Player, rotationSpeed);
            var dieState = new FlyingEnemyDieState(this, animator); 
            
       
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
        
        void Update() 
        {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }
        void FixedUpdate() 
        {
            stateMachine.FixedUpdate();
        }
        
   

        public void Attack()
        {
            if (attackTimer.IsRunning) return;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyAttack, this.transform.position);
            attackTimer.Start();

            // Access PlayerHealth and apply damage only if the player is not invulnerable
            var playerHealth = playerDetector.Player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (!playerHealth.IsInvulnerable)
                {
                    // Assuming you add a public IsInvulnerable property
                    playerHealth.TakeDamage(damageAmount);
                }
                else
                {
                    Debug.Log("Player is invulnerable. Attack did no damage.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the player!");
            }
        }
    }
}