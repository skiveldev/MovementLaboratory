using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayers;

    private float horizontalInput;
    private bool jumpRequested;

    private void Awake()
    {
        playerBody ??= GetComponent<Rigidbody2D>();
        animator ??= GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Read player input once per rendered frame.
        horizontalInput = Input.GetAxisRaw("Horizontal");
        jumpRequested |= Input.GetButtonDown("Jump");

        if (horizontalInput != 0f && visual != null)
        {
            visual.flipX = horizontalInput < 0f;
        }

        DriveAnimator();
    }

    private void FixedUpdate()
    {
        // Apply Rigidbody2D changes on the fixed physics step.
        var velocity = playerBody.linearVelocity;
        playerBody.linearVelocity = new Vector2(horizontalInput * moveSpeed, velocity.y);

        if (jumpRequested && IsGrounded())
        {
            playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        jumpRequested = false;
    }

    private bool IsGrounded()
    {
        return groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers) != null;
    }

    // The controller blends Idle/Run/Jump/Fall from these parameters.
    private void DriveAnimator()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("Grounded", IsGrounded());
        animator.SetFloat("VelocityY", playerBody != null ? playerBody.linearVelocity.y : 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
