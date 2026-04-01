using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float moveSpeed = 10f;
    public float jumpForce = 16f;

    [Header("Mecánicas de Pared (Wall Slide)")]
    public float wallSlidingSpeed = 1.5f; // Cuanto más bajo, más lento cae
    public Vector2 wallJumpPower = new Vector2(12f, 18f);
    public float wallJumpDuration = 0.2f;

    [Header("Detección y Capas")]
    public LayerMask groundLayer;
    public Transform wallCheck; 
    public float wallCheckRadius = 0.35f;

    private Rigidbody2D rb;
    private Collider2D coll;
    private float horizontalInput;
    private bool isFacingRight = true;
    
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        
        // TRUCO PRO: Quitamos la fricción por código para que no se "trabe"
        if (coll.sharedMaterial == null) {
            PhysicsMaterial2D mat = new PhysicsMaterial2D("SlideMaterial");
            mat.friction = 0f;
            coll.sharedMaterial = mat;
        }
    }

    void Update()
    {
        if (isWallJumping) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Lógica de Salto Normal
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        HandleWallSlide();
        HandleWallJump();
        
        if (!isWallSliding)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (isWallJumping) return;

        // Movimiento horizontal fluido
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleWallSlide()
    {
        // Si tocamos pared, no estamos en el suelo y nos movemos hacia la pared
        if (IsWalled() && !IsGrounded() && horizontalInput != 0)
        {
            isWallSliding = true;
            // Forzamos la velocidad de caída lenta
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x; // Salta hacia el lado opuesto a donde mira
            wallJumpTimer = 0.15f; // Tiempo de gracia
            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;

            // Girar al personaje al saltar de la pared
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    private void StopWallJump() => isWallJumping = false;

    private bool IsGrounded() => Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
    }
}