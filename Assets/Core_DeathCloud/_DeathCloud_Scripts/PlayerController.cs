using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float moveSpeed = 10f;
    public float jumpForce = 16f;

    [Header("Mecánicas de Pared (Wall Slide)")]
    public float wallSlidingSpeed = 1.5f;
    public Vector2 wallJumpPower = new Vector2(12f, 18f);
    public float wallJumpDuration = 0.2f;

    [Header("Detección y Capas")]
    public LayerMask groundLayer;
    public Transform wallCheck; 
    public float wallCheckRadius = 0.35f;
    
    // --- NUEVO: SENSOR DE SUELO INDEPENDIENTE ---
    [Header("Sensor de Suelo (Para Rampas)")]
    public Transform groundCheck; // Arrastra aquí tu objeto vacío "Pie_Sensor"
    public float groundCheckRadius = 0.3f; 

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
        
        // Aplicamos fricción cero para no trabarnos
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

        // Lógica de Salto usando el NUEVO sensor
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
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleWallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontalInput != 0)
        {
            isWallSliding = true;
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
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = 0.15f;
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

    // --- CAMBIO CLAVE: Ahora detecta el suelo desde el objeto vacío ---
    private bool IsGrounded() 
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

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
        // Dibujamos el sensor de pared en ROJO
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }

        // Dibujamos el sensor de suelo en AMARILLO
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}