using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;
using UnityEngine.Rendering.Universal;
using FMOD.Studio;
using Timer = Utilities.Timer;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour , IDataPersistence
    {
        [Header("References")] 
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Self] private FootstepController footstepController;
        [SerializeField, Self] PlayerHealth playerHealth;
        [SerializeField, Self] GlideStamina glideStamina;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;
        
        [Header("Jump Settings")] 
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] private int jumpCount = 2;
        [SerializeField] private int remainingJumps = 2;
        [SerializeField] private int maxFallSpeed = 10;

        [Header("Dash Settings")] 
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;
        
        [Header("Echolocation Settings")] 
        [SerializeField] float echoCooldown = 0.5f;
        [SerializeField] float detectionRadius = 5f; 
        public LayerMask detectionLayer;
        [SerializeField] ParticleSystem detectionParticle;
        public ScriptableRendererFeature echoRendererFeature; // Reference to your custom RenderObjects renderer feature
        private ScriptableRendererData rendererData;
      
        
        [Header("Glide Settings")] 
        public float glideBoost = 1;
        private float glideBoostDecayRate = 0.02f;
        [SerializeField] float glideFallSpeed = 0.1f;
        public float glideTime = 3;
        
        [Header("Attack Settings")]
        [SerializeField] float attackCoolDown = 0.5f;
        [SerializeField] float attackDistance = 1f;
        [SerializeField] private float spinAttackDistance = 5f;
        [SerializeField] int attackDamage = 10;
        [SerializeField] int spinAttackDamage = 20;
        [SerializeField] private float knockbackTime = 0.5f;
        
        //portal
        [Header("Interact")] 
        [SerializeField] private float interactDistance = 5;

        [Header("Wall Climb Settings")] 
        [SerializeField] private float wallCheckDist = 1f;
        [SerializeField] private float wallClimbMoveSpeed = 5f;
        [SerializeField] private LayerMask wallClimbLayer;
        
        public bool wallClimbimg;
        bool[] wallClimbChecks;
        private Vector3 wallClimbNormal;
        private Vector3 wallClimbTargetPos;
        public Vector3 wallClimbPos;

        const float ZeroF = 0f;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;
        float dashVelocity = 1f;

        Vector3 movement;

        [Header("Timers")] List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;
        CountdownTimer attackTimer;
        CountdownTimer spinAttackTimer;
        CountdownTimer echoTimer;
        CountdownTimer glideTimer;
        
        public bool isTeleporting;

        StateMachine stateMachine;
        //Audio
        private EventInstance playerFootsteps;
        
        // Animator parameter
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            // Invoke event when observed transform is teleported, adjusting freeLookVCam's position accordingly
            freeLookVCam.OnTargetObjectWarped(transform,
                transform.position - freeLookVCam.transform.position - Vector3.forward);
            rb.freezeRotation = true;

            glideStamina = GetComponent<GlideStamina>();

            SetupTimers();
            SetupStateMachine();
        }
        
        public void LoadData(GameData data) 
        {
            this.transform.position = data.playerPosition;
        }

        public void SaveData(GameData data) 
        {
            data.playerPosition = this.transform.position;
        }

        void Start()
        {
            playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.playerFootsteps);
            input.EnablePlayerActions();
            
        }
        void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            stateMachine.Update();

            HandleTimers();
            TriggerFootstepEvents();
            UpdateAnimator();
            UpdateSound();
        }
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
            WallClimbCheck();
        }
        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }
        
        private void SetupStateMachine()
        {
            // State Machine
            stateMachine = new StateMachine();

            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var glideState = new GlideState(this, animator);
            var dashState = new DashState(this, animator);
            var attackState = new AttackState(this, animator);
            var spinAttackState = new SpinAttackState(this, animator);
            var deathState = new DeathState(this, animator);

            var doubleJumpState = new DoubleJumpState(this, animator);
            var echoLocationState = new EcholocationState(this, animator);
            var teleportState = new TeleportState(this, animator);
            var wallClimbState = new WallClimbState(this, animator);
            
            // Define transitions for jump 
            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(locomotionState, jumpState, new FuncPredicate(() => !groundChecker.IsGrounded));

            // Define transitions for double jump
            At(doubleJumpState, glideState, new FuncPredicate(() => glideTimer.IsRunning));
            At(jumpState, doubleJumpState, new FuncPredicate(() => jumpTimer.IsRunning && remainingJumps <= jumpCount - 2));
            At(doubleJumpState, dashState, new FuncPredicate(() => dashTimer.IsRunning));

            // Define transitions for Dash 
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(glideState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(jumpState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(dashState, jumpState, new FuncPredicate(() => !dashTimer.IsRunning));
            
            // Define transitions for attack
            At(locomotionState, attackState, new FuncPredicate(() => attackTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(() => !attackTimer.IsRunning));
            
            At(locomotionState, spinAttackState, new FuncPredicate(() => spinAttackTimer.IsRunning));
            At(spinAttackState, locomotionState, new FuncPredicate(() => !spinAttackTimer.IsRunning));
            
            // Enable Spin Attack while jumping
            At(jumpState, spinAttackState, new FuncPredicate(() => spinAttackTimer.IsRunning));
            At(spinAttackState, jumpState, new FuncPredicate(() => !spinAttackTimer.IsRunning || jumpTimer.IsRunning));

            // Define transitions to echo state
            At(locomotionState, echoLocationState, new FuncPredicate(() => echoTimer.IsRunning));
            At(echoLocationState, locomotionState, new FuncPredicate(() => !echoTimer.IsRunning));

            // Define transitions for glide
            At(jumpState, glideState, new FuncPredicate(() => glideTimer.IsRunning));
            At(dashState, glideState, new FuncPredicate(() => glideTimer.IsRunning));
            At(glideState, jumpState, new FuncPredicate(() => !glideTimer.IsRunning));
            
            // Define transitions for wall climb
            At(jumpState, wallClimbState, new FuncPredicate(() => wallClimbimg));
            At(doubleJumpState, wallClimbState, new FuncPredicate(() => wallClimbimg));
            At(wallClimbState, doubleJumpState, new FuncPredicate(() => !wallClimbimg));
            
            // Definne transition for teleportation
             At(teleportState, locomotionState, new FuncPredicate(() => !isTeleporting));

            // Set initial state
            Any(teleportState, new FuncPredicate(() => isTeleporting));
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));
            Any(deathState, new FuncPredicate(() => playerHealth.currentHealth <= 0));

            stateMachine.SetState(locomotionState);
            playerHealth.OnDeath += () => stateMachine.SetState(deathState);
        }

        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !attackTimer.IsRunning
                   && !spinAttackTimer.IsRunning
                   && !jumpTimer.IsRunning
                   && !dashTimer.IsRunning
                   && !glideTimer.IsRunning
                   && !echoTimer.IsRunning
                   && !isTeleporting;

        }
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            echoTimer = new CountdownTimer(echoCooldown);
            glideTimer = new CountdownTimer(glideTime);
            attackTimer = new CountdownTimer(attackCoolDown);
            spinAttackTimer = new CountdownTimer(attackCoolDown);

            timers = new List<Timer>(8)
                { jumpTimer, jumpCooldownTimer, dashTimer, dashCooldownTimer, attackTimer,spinAttackTimer, echoTimer, glideTimer };
        }
        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection * glideBoost);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for a snappy stop
                rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
            }
        }
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move player
            Vector3 velocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
        void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement Direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
           // transform.LookAt(transform.position + adjustedDirection);
        }
        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
        void OnWallClimb(bool performed)
        {
            if (performed)
            {
                if (wallClimbimg)
                {
                    wallClimbimg = false;
                    if (remainingJumps > 0)
                    {
                        OnJump(true);
                    }
                    else
                    {
                        remainingJumps++;
                        OnJump(true);
                    }
                }
                else
                {
                    if (wallClimbChecks[5])
                    {
                        wallClimbimg = true;
                    }
                }
            }
        }
        public void HandleWallClimb()
        {
            //movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

            rb.velocity = Vector3.zero;
            transform.position = wallClimbPos;

            transform.LookAt(transform.position - wallClimbNormal);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

            if (movement.magnitude > 0)
            {
                if (movement.z > 0)
                {
                    wallClimbPos += transform.up * 0.01f * wallClimbMoveSpeed;
                }
                else if (movement.z < 0)
                {
                    wallClimbPos -= transform.up * 0.01f * wallClimbMoveSpeed;
                }
         
                if (movement.x > 0)
                {
                    wallClimbPos += transform.right * 0.01f * wallClimbMoveSpeed;
                    wallClimbPos += transform.right * 0.01f * wallClimbMoveSpeed;
                }
                else if (movement.x < 0)
                {
                    wallClimbPos -= transform.right * 0.01f * wallClimbMoveSpeed;
                }

                if (!wallClimbChecks[5] && ! wallClimbChecks[2])
                {
                    wallClimbimg = false;
                }
            }

            currentSpeed = movement.magnitude;
        }
        /// <summary>
        /// Wallclimb Checks go in this order:
        /// 0 = spherecast in front to see if wallclimbing can be triggered
        /// 1 = above
        /// 2 = below
        /// 3 = right
        /// 4 = left
        /// 5 = raycast to find normal of wall found in check #0
        /// </summary>
        void WallClimbCheck()
        {
            wallClimbChecks = new bool[6];
            for (int i = 0; i < wallClimbChecks.Length; i++)
            {
                wallClimbChecks[i] = false;
            }

            wallClimbChecks[0] = (Physics.SphereCastAll(
                transform.position + transform.forward * wallCheckDist + transform.up, 
                0.5f,
                transform.forward,
                0.1f,
                wallClimbLayer).Length > 0);

            wallClimbChecks[1] = Physics.Raycast(transform.position + transform.up * 2, transform.forward, 1, wallClimbLayer);
            wallClimbChecks[2] = Physics.Raycast(transform.position - transform.up * 0.6f, transform.forward, 1, wallClimbLayer);
    
            //wallClimbChecks[3] = Physics.Raycast(transform.position + transform.right * 1 + transform.up * 0.5f, transform.forward, 1, wallClimbLayer);
            //wallClimbChecks[4] = Physics.Raycast(transform.position - transform.right * 1 + transform.up * 0.5f, transform.forward, 1, wallClimbLayer);

            if (wallClimbChecks[0])
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out hit, 1, wallClimbLayer))
                {
                    wallClimbChecks[5] = true;
                    wallClimbTargetPos = hit.point;
                    wallClimbNormal = hit.normal;
                }
       
            }
            else
            {
                wallClimbNormal = Vector3.zero;
                wallClimbTargetPos = Vector3.zero;
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * attackDistance, attackDistance);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, spinAttackDistance);
     
            if (Application.isPlaying)
            {
                Gizmos.color = Color.grey;
                if(wallClimbChecks[0]) Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + transform.forward * wallCheckDist + transform.up, 0.5f);
         
                Gizmos.color = Color.yellow;
                if(wallClimbChecks[1]) Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + transform.up * 2, transform.forward * 1);
         
                /*
                Gizmos.color = Color.yellow;
                if(wallClimbChecks[2]) Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position - transform.up * 0.6f, transform.forward * 1);
         
                Gizmos.color = Color.yellow;
                if(wallClimbChecks[3]) Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + transform.right * 1 + transform.up * 0.5f, transform.forward);
                */
                
                Gizmos.color = Color.yellow;
                if(wallClimbChecks[4]) Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position - transform.right * 1 + transform.up * 0.5f, transform.forward);
         
                Gizmos.color = Color.magenta;
                if(wallClimbNormal != Vector3.zero) Gizmos.DrawRay(wallClimbTargetPos, wallClimbNormal);
                
   
            }
        }
        void OnInteract(bool performed)
        {
            if(performed)
            {
                Debug.Log("Trying to interact");
        
                foreach (var interactable in FindObjectsByType<Interactable>(FindObjectsSortMode.InstanceID))
                {
                    if (Vector3.Distance(transform.position, interactable.transform.position) < interactDistance)
                    {
                        interactable.Interact();
                        break;
                    }
                }
            }
        }
        void OnAttack()
        {
            if (!attackTimer.IsRunning)
            {
                attackTimer.Start();
            }
        }
        public void Attack()
        {
            Vector3 attackPos = transform.position + transform.forward * attackDistance;
            Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerAttack, this.transform.position);
            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage, knockbackTime);
                }
            }
        }
        void OnSpinAttack()
        {
            if (!spinAttackTimer.IsRunning)
            {
                spinAttackTimer.Start();
            }
        }
        public void SpinAttack()
        {
            Vector3 attackPos = transform.position;
            Collider[] hitEnemies = Physics.OverlapSphere(attackPos, spinAttackDistance);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerAttack, this.transform.position);
            foreach (var enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<EnemyHealth>().TakeDamage(spinAttackDamage, knockbackTime);
                }
            }
        }
        void OnEcho(bool performed)
        {
            if (!echoTimer.IsRunning)
            {
                echoTimer.Start();
            }
        }

        public void HandleEcho()
        {
            if (echoCooldown > 0 && echoTimer.IsRunning)
            {
                detectionParticle.Play(); 
                // Detect objects within the defined radius
                Collider[] detectedObjects = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
                foreach (Collider collider in detectedObjects)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        // Dynamically enable the render feature for the duration of the echo
                        EnableEchoEffect(true);
                    } 
                    if (collider.CompareTag("Collectible"))
                    {
                        EnableEchoEffect(true);
                    }
                }
                // Disable the effect after a delay (e.g., for a short duration)
                Invoke(nameof(DisableEchoEffect), 5f); // Adjust time if needed
            }
        } 
        private void EnableEchoEffect(bool state)
        {
            if (echoRendererFeature != null)
            {
                echoRendererFeature.SetActive(state);
            }
        }
        private void DisableEchoEffect()
        {
            EnableEchoEffect(false);
        }
        void OnDash(bool performed)
        {
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning && !glideTimer.IsRunning)
            {
                dashTimer.Start();
                glideStamina.StartGlide();
            }
            else if (!performed && dashTimer.IsRunning)
            {
                dashTimer.Stop();
                glideStamina?.StopGlide();
            }
        }
        void OnJump(bool performed)
        {
            if (wallClimbimg)
            {
                OnWallClimb(true);
                return;
            }
            
            if (performed && groundChecker.IsGrounded)
            {
                remainingJumps = jumpCount;
            }

            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && remainingJumps > 0)
            {
                remainingJumps--;
                jumpTimer.Start();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerJump, this.transform.position);
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }
        public void HandleJump()
        {
            // if not jumping and grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                //jumpTimer.Stop();
                return;
            }

            // if jumping or falling calculate velocity
            if (!jumpTimer.IsRunning)
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
                jumpVelocity = Mathf.Clamp(jumpVelocity, -maxFallSpeed, maxFallSpeed);
            }

            // Apply velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
        public void OnGlide(bool performed)
        {
            if (performed)
            {
                if (!glideTimer.IsRunning && !groundChecker.IsGrounded)
                {
                        glideTimer.Start();
                        glideBoost = 0;
                        jumpTimer.Stop();
                        glideStamina.StartGlide();
                }
            }
            else if (!performed && glideTimer.IsRunning)
            {
                glideStamina?.StopGlide();
                glideTimer.Stop();
            }
        }
        public void HandleGlide()
        {
            if (glideBoost > 1)
            {
                glideBoost -= glideBoostDecayRate;
                rb.velocity = new Vector3(rb.velocity.x * glideBoost, -glideFallSpeed, rb.velocity.z * glideBoost);
            }
            else
            {
                glideBoost = 1;
                rb.velocity = new Vector3(rb.velocity.x, -glideFallSpeed, rb.velocity.z);
            }
            if (groundChecker.IsGrounded)
            {
                glideTimer.Stop();
                glideStamina?.StopGlide();
            }
        }
        void TriggerFootstepEvents()
        {
            if (groundChecker.IsGrounded && currentSpeed > 0.1f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                {
                    // Trigger left foot during certain frames
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.5f)
                        footstepController.LeftFootDown();
                    else
                        footstepController.RightFootDown();
                }
            }
        }
        void UpdateSound()
        {
            // start footsteps event if the player has an x velocity and is on the ground
            if (rb.velocity.x != 0 && groundChecker.IsGrounded)
            {
                // get the playback state
                PLAYBACK_STATE playbackState;
                playerFootsteps.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    playerFootsteps.start();
                }
            }
            // otherwise, stop the footstep event
            else
            {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }
        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
            input.Echo += OnEcho;
            input.Wallclimb += OnWallClimb;
            input.Glide += OnGlide;
            input.Attack += OnAttack;
            input.SpinAttack += OnSpinAttack;
            input.interact += OnInteract;
            
        }
        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
            input.Echo -= OnEcho;
            input.Wallclimb -= OnWallClimb;
            input.Glide -= OnGlide;
            input.Attack -= OnAttack;
            input.SpinAttack -= OnSpinAttack;
            input.interact -= OnInteract;
            
        }
    }
}