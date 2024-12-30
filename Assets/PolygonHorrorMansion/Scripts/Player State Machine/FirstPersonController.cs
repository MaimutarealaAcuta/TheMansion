using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FirstPersonController : MonoBehaviour, IDamageable
{
    public bool CanMove { get; set; } = true;
    private bool IsSprinting => canSprint & Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) & !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] public bool canSprint = true;
    [SerializeField] public bool canJump = true;
    [SerializeField] public bool canCrouch = true;
    [SerializeField] public bool canUseHeadbob = true;
    [SerializeField] public bool canSlideOnSlopes = true;
    [SerializeField] public bool canInteract = true;
    [SerializeField] public bool useFootsteps = true;
    [SerializeField] public bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 3;
    [SerializeField] public float currentHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaUseMultiplier = 5;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 4;
    [SerializeField] private float staminaValueIncrement = 2;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;
    public static Action OnPlayerEscaped;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = .04f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = .1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = .02f;
    private float defaultYPos = 8;
    private float timer;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] gravelClips = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    // SLIDING PARAMETERS
    private Vector3 hitPointNormal;
    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded & Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;
    public Transform holdPoint;

    [Header("UI Elements")]
    public TextMeshProUGUI stunMessageText;

    [HideInInspector] public CharacterController characterController;
    private Camera playerCamera;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;
    private IPlayerState currentState;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        defaultYPos = playerCamera.transform.localPosition.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set initial state
        SetPlayerState(new NeutralState());
    }

    void Update()
    {
        currentState.UpdateState();
    }

    public void SetPlayerState(IPlayerState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;
        currentState.EnterState(this);
    }

    public void HandleMovementInput()
    {
        float movementSpeed = isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
        currentInput = new Vector2(movementSpeed * Input.GetAxis("Vertical"), movementSpeed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;

        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        float avoidFloorDistance = -0.8f;
        float ladderGrabDistance = 0.7f;

        var direction = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

        if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, direction, out RaycastHit raycastHit, ladderGrabDistance))
        {
            if (raycastHit.transform.TryGetComponent(out Ladder ladder))
            {
                moveDirection.y = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude;
                moveDirection.x = 0;
                moveDirection.z = 0;
            }
        }
    }

    public void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    public void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    public void HandleHeadbob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);

            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    public void HandleFootsteps()
    {
        // No sound if you aren't grounded or not moving at all
        if (!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

            if (Physics.Raycast(characterController.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Wood":
                        footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length - 1)]);
                        break;
                    case "Footsteps/Grass":
                        footstepAudioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length - 1)]);
                        break;
                    case "Footsteps/Gravel":
                        footstepAudioSource.PlayOneShot(gravelClips[UnityEngine.Random.Range(0, gravelClips.Length - 1)]);
                        break;
                    default:
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }

    public void HandleInteractionCheck()
    {
        // Raycast to find what we hit
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            // Attempt to get the Interactable from the hit
            bool isInteractableLayer = hit.collider.gameObject.layer == 7; // Interactable layer
            Interactable newInteractable = null;

            if (isInteractableLayer)
            {
                // Try to get the Interactable component
                hit.collider.TryGetComponent(out newInteractable);
            }

            // If the new interactable is different from our current one,
            // we handle the lose focus on the old, and the focus on the new.
            if (newInteractable != currentInteractable)
            {
                // If we already had one focused, lose focus first
                if (currentInteractable != null)
                {
                    currentInteractable.OnLoseFocus();
                }

                // Update to the new interactable (could be null if we didn't find anything)
                currentInteractable = newInteractable;

                // Focus the new one if it's valid
                if (currentInteractable != null)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else
        {
            // If we didn't hit anything, but we currently have an interactable, lose focus
            if (currentInteractable != null)
            {
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
            }
        }
    }


    public void HandleInteractionInput()
    {
        bool isInteracting = Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer);
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && isInteracting)
        {
            currentInteractable.OnInteract();
        }
    }

    /*
    * Vertical movement of the mouse controls the X axis of the Camera.
    * Horizontal movement of the mouse controls the Y axis of the Character.
    * 
    * While it may seem you need to move the camera on the horizontal axis,
    * you actually need to turn the body of the character, otherwise the
    * camera will rotate around the body making it look like you dont move
    * at all from another person's perspective.
    */
    public void HandleMouseLook()
    {
        if (!CanMove) return;

        // Look Up/Down
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Look Left/Right
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    public void ApplyFinalMovements()
    {
        if (!CanMove) return;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (canSlideOnSlopes && IsSliding)
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        // Do not allow standing if something is above you
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure we have the exact targeted values
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            SetPlayerState(new DeadState());
        }
        else
        {
            SetPlayerState(new StunnedState());
        }
    }

    public void UnstunPlayer()
    {
        SetPlayerState(new NeutralState());
        OnPlayerEscaped?.Invoke();
    }

    public void HandleStamina()
    {
        if (IsSprinting && currentInput != Vector2.zero)
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            
            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0)
            {
                currentStamina = 0;
            }

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
            {
                canSprint = false;
            }
        }

        if (!IsSprinting & currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }

        regeneratingStamina = null;
    }
}
