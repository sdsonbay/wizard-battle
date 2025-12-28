using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    public Animator animator;
    public Transform cameraTransform;

    public GameObject spellPrefab;
    public Transform spellSpawnPoint;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool isWalking = false;
    private bool isRunning = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isAttacking)
        {
            HandleInput();
            HandleRotation();
        }

        HandleAnimation();
        HandleAttack();
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            Move();
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        moveDirection = (camForward * v + camRight * h).normalized;

        isWalking = moveDirection.magnitude > 0.1f;
        isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetPos = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    void HandleRotation()
    {
        if (isWalking)
        {
            Vector3 lookDir = moveDirection;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void HandleAnimation()
    {
        animator.SetBool("isWalking", isWalking && !isRunning && !isAttacking);
        animator.SetBool("isRunning", isRunning && !isAttacking);
        animator.SetBool("isAttack", isAttacking);
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            SpawnSpell();
            animator.SetBool("isAttack", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            moveDirection = Vector3.zero;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (isAttacking && stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.95f)
        {
            isAttacking = false;
            animator.SetBool("isAttack", false);
        }
    }

    public void SpawnSpell()
    {
        Instantiate(spellPrefab, spellSpawnPoint.position, Quaternion.LookRotation(cameraTransform.forward));
    }
}