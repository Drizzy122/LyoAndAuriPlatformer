using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;
using FMOD.Studio;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour, IDataPersistence
    {
        [Header("References")] [SerializeField, Self]
        Rigidbody rb;

        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] FootstepController footstepController;
        [SerializeField, Self] PlayerHealth playerHealth;

        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")] [SerializeField]
        float moveSpeed = 6f;

        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")] [SerializeField]
        float jumpForce = 10f;

        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] private int jumpCount = 2;
        private int remainingJumps = 2;

        [Header("Glide Settings")] [SerializeField]
        private float glideFallSpeed = 0.1f;

        [SerializeField] private float glideTime = 3;


        [Header("Dash Settings")] [SerializeField]
        float dashForce = 10f;

        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;

        [Header("Attack Settings")] [SerializeField]
        float attackCooldown = 0.5f;

   
        //[SerializeField] float knockbackForce = 10f;
        [SerializeField] float attackDistance = 1f;
        [SerializeField] int attackDamage = 10;


        const float ZeroF = 0f;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;
        float dashVelocity = 1f;

        Vector3 movement;

        [Header("Timers, dont touch")] 
        private List<Timer> timers;
        private CountdownTimer jumpTimer;
        private CountdownTimer jumpCooldownTimer;
        private CountdownTimer glideTimer;
        private CountdownTimer dashTimer;
        private CountdownTimer dashCooldownTimer;
        private CountdownTimer attackTimer;

        StateMachine stateMachine;

        //Audio
        private EventInstance playerFootsteps;

        // Animator parameters
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

            SetupTimers();
            SetupStateMachine();
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
            UpdateAnimator();
            TriggerFootstepEvents();
            UpdateSound();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void LoadData(GameData data)
        {
            //Debug.Log("Loading player position: " + data.playerPosition.ToString());
            this.transform.position = data.playerPosition;
        }

        public void SaveData(GameData data)
        {
            // Debug.Log("Saving player position: " + this.transform.position.ToString());
            data.playerPosition = this.transform.position;
        }

        void SetupStateMachine()
        {
            // State Machine
            stateMachine = new StateMachine();

            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var glideState = new GlideState(this, animator);
            var dashState = new DashState(this, animator);
            var attackState = new AttackState(this, animator);
            var deathState = new DeathState(this, animator);
            
            // Add new states 
            var doubleJumpState = new DoubleJumpState(this, animator); // Add DoubleJumpState
            var echoLocationState = new EcholocationState(this, animator); // Add EcholocationState
            var wallClimbState = new WallClimbState(this, animator); // Add WallClimbState

            // Define transitions
            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(locomotionState, jumpState, new FuncPredicate(() => !groundChecker.IsGrounded));

            At(jumpState, glideState, new FuncPredicate(() => glideTimer.IsRunning));
            At(glideState, jumpState, new FuncPredicate(() => !glideTimer.IsRunning));
            
            At(locomotionState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
            At(locomotionState, attackState, new FuncPredicate(() => attackTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(() => !attackTimer.IsRunning));

            // add new transitions
           
            
            
            
            
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));
            Any(deathState, new FuncPredicate(() => playerHealth.currentHealth <= 0));
            
            playerHealth.OnDeath += () => stateMachine.SetState(deathState);
            // Set initial state
            stateMachine.SetState(locomotionState);
        }

        bool ReturnToLocomotionState()
        {
            return groundChecker.IsGrounded
                   && !attackTimer.IsRunning
                   && !jumpTimer.IsRunning
                   && !dashTimer.IsRunning
                   && !glideTimer.IsRunning;
        }

        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            glideTimer = new CountdownTimer(glideTime);

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            dashTimer.OnTimerStart += () => dashVelocity = dashForce;
            dashTimer.OnTimerStop += () =>
            {
                dashVelocity = 1f;
                dashCooldownTimer.Start();
            };

            attackTimer = new CountdownTimer(attackCooldown);

            timers = new(5) { jumpTimer, jumpCooldownTimer, glideTimer, dashTimer, dashCooldownTimer, attackTimer };
        }

        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void OnEnable()
        {
            input.Jump += OnJump;
            input.Dash += OnDash;
            input.Attack += OnAttack;
            input.Glide += OnGlide;
            input.Echo += OnEcholocation;
        }

        void OnDisable()
        {
            input.Jump -= OnJump;
            input.Dash -= OnDash;
            input.Attack -= OnAttack;
            input.Glide -= OnGlide;
            input.Echo -= OnEcholocation;
        }

        void OnEcholocation()
        {
            
        }

        void OnAttack()
        {
            if (!attackTimer.IsRunning)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerAttack, this.transform.position);
                attackTimer.Start();
            }
        }
        void OnJump(bool performed)
        {
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

        void OnDash(bool performed)
        {
            if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning)
            {
                dashTimer.Start();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerSpin, this.transform.position);
            }
            else if (!performed && dashTimer.IsRunning)
            {
                dashTimer.Stop();
            }
        }

        void OnGlide(bool performed)
        {
            if (performed)
            {

                if (!glideTimer.IsRunning //&& jumpTimer.IsRunning
                    && !groundChecker.IsGrounded)
                {
                    //print("Glide Started");
                    glideTimer.Start();
                    jumpTimer.Stop();
                }
            }
            else if (!performed && glideTimer.IsRunning)
            {
                //print("Glide Stopped");
                glideTimer.Stop();
            }
        }

        void UpdateAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void Echolocation()
        {
            // Track enemies location 
            // Play audio 
            // VFX 
        }

        public void WallClimb()
        {
            // wall climb and jump off 
        }
        

        public void Attack()
        {
            Vector3 attackPos = transform.position + transform.forward;
            Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);

            foreach (var enemy in hitEnemies)
            {
                //Debug.Log(enemy.name);
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
                    // add the knockback
                    
                }
            }
        }
        public void HandleJump()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                return;
            }

            if (!jumpTimer.IsRunning)
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            // Apply velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }

        public void HandleGlide()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -glideFallSpeed, rb.linearVelocity.z);

            if (groundChecker.IsGrounded) glideTimer.Stop();
        }

        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);

            }
            else
            {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for a snappy stop
                rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);

            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            Vector3 velocity = adjustedDirection * (moveSpeed * dashVelocity * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
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
            if (rb.linearVelocity.x != 0 && groundChecker.IsGrounded)
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
    }
}