using System.Collections;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private enum HookState { Inactive, Throwing, Hooked, Retracting }
    private HookState currentState = HookState.Inactive;

    [Header("Configuración del Radar")]
    public float maxDistance = 20f;
    public LayerMask hookableLayer;

    [Header("Punto de Mira / Crosshair")]
    public SpriteRenderer cursorVisual; 
    public Color crosshairColorNormal = Color.white;
    public Color crosshairColorValid = Color.cyan;

    [Header("Pulido Visual Cuerda")]
    public Transform firePoint; 
    public Color normalColor = Color.white;
    public Color tensionColor = Color.red; 

    [Header("Mecánica de Ruptura INEVITABLE")]
    // AUMENTADO: 5 segundos al máximo de tensión garantizan una oscilación completa (ida y vuelta).
    public float snappingTimeSlack = 12.0f; 
    public float snappingTimeTense = 5.0f; 
    private float tensionStress = 0f;      

    [Header("Físicas y Fluidez de Balanceo")]
    public float swingForce = 40f;   
    public float maxSpeedLimit = 25f;

    [Header("Escalada Realista")]
    public float staticClimbSpeed = 12f;  
    public float swingingClimbSpeed = 1.5f; 

    [Header("Salto Spider-Man 2")]
    public float normalJumpPop = 16f; 
    public float normalJumpHorizontalControl = 12f; 
    public float horizontalPreservationMultiplier = 1.3f; 
    public float verticalPopForce = 15f; 
    public float perfectTimingMultiplier = 1.6f; 

    [Header("Velocidades Ágiles")]
    public float hookThrowSpeed = 90f;  
    public float hookReturnSpeed = 140f; 

    [Header("Efectos y Feedback")]
    public ParticleSystem shootParticles;
    public ParticleSystem snapParticles;
    public AudioSource audioSource;
    public AudioClip shootSFX;
    public AudioClip snapSFX;

    private Camera mainCamera;
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private PlayerController playerController;
    
    private PhysicsMaterial2D defaultMaterial;
    private PhysicsMaterial2D frictionlessMaterial;

    private float horizontalInput;
    private float verticalInput;
    private Vector2 currentMousePos;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>(); 

        defaultMaterial = playerCollider.sharedMaterial;
        frictionlessMaterial = new PhysicsMaterial2D("Frictionless");
        frictionlessMaterial.friction = 0f;

        // FIX: Buscamos los componentes que ya tienes en el Inspector para no duplicarlos
        distanceJoint = GetComponent<DistanceJoint2D>();
        if (distanceJoint == null) distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
        
        distanceJoint.enabled = false;
        distanceJoint.enableCollision = true;
        distanceJoint.maxDistanceOnly = true; 
        
        // FIX CRÍTICO: Evita que Unity mueva el gancho a la coordenada (0,0) por defecto
        distanceJoint.autoConfigureConnectedAnchor = false; 

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
        
        lineRenderer.positionCount = 2;
        // Solo asignamos el material si no tiene uno puesto en el Inspector
        if (lineRenderer.material == null || lineRenderer.material.name == "Default-Material")
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        lineRenderer.enabled = false;

        if (cursorVisual == null)
        {
            GameObject go = new GameObject("PuntoDeMira");
            cursorVisual = go.AddComponent<SpriteRenderer>();
            Texture2D tex = new Texture2D(32, 32);
            for(int y=0; y<32; y++) for(int x=0; x<32; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                tex.SetPixel(x, y, d < 12 && d > 10 ? Color.white : Color.clear);
            }
            tex.Apply();
            cursorVisual.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
        Cursor.visible = false;
    }
    private Vector2 GetOrigin()
    {
        return firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
    }

    void Update()
    {
        currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        cursorVisual.transform.position = currentMousePos;

        if (currentState == HookState.Inactive)
        {
            Vector2 origin = GetOrigin();
            Vector2 direction = (currentMousePos - origin).normalized;
            RaycastHit2D previewHit = Physics2D.Raycast(origin, direction, maxDistance, hookableLayer);

            if (previewHit.collider != null)
            {
                cursorVisual.color = crosshairColorValid;
                cursorVisual.transform.localScale = Vector3.one * 1.2f; 
            }
            else
            {
                cursorVisual.color = crosshairColorNormal;
                cursorVisual.transform.localScale = Vector3.one;
            }
        }
        else
        {
            cursorVisual.color = crosshairColorNormal * 0.5f; 
        }

        if (Input.GetMouseButtonDown(1) && currentState == HookState.Inactive)
        {
            StartCoroutine(ThrowHookRoutine());
        }
        else if (Input.GetMouseButtonUp(1) && (currentState == HookState.Hooked || currentState == HookState.Throwing))
        {
            ReleaseHook(false); 
        }

        if (currentState == HookState.Hooked)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical"); 

            lineRenderer.SetPosition(0, GetOrigin());
            lineRenderer.SetPosition(1, distanceJoint.connectedAnchor);

            float currentDist = Vector2.Distance(GetOrigin(), distanceJoint.connectedAnchor);
            float tensionPercent = Mathf.Clamp01(currentDist / maxDistance);

            float currentTimeToBreak = Mathf.Lerp(snappingTimeSlack, snappingTimeTense, tensionPercent);
            tensionStress += Time.deltaTime / currentTimeToBreak;

            // El parpadeo ahora es más suave al principio y muy agresivo al final
            float blinkSpeed = Mathf.Lerp(2f, 30f, Mathf.Pow(tensionStress, 2)); 
            float redIntensity = Mathf.PingPong(Time.time * blinkSpeed, 1f) * tensionStress;
            Color blinkColor = Color.Lerp(normalColor, tensionColor, redIntensity);
            
            float width = Mathf.Lerp(0.1f, 0.02f, tensionPercent);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = blinkColor;
            lineRenderer.endColor = blinkColor;

            if (tensionStress >= 1f)
            {
                ReleaseHook(false, true); 
                return; 
            }

            if (verticalInput != 0)
            {
                float speedFactor = Mathf.InverseLerp(0f, maxSpeedLimit * 0.7f, rb.linearVelocity.magnitude);
                float activeClimbSpeed = Mathf.Lerp(staticClimbSpeed, swingingClimbSpeed, speedFactor);
                if (verticalInput < 0) activeClimbSpeed = Mathf.Max(activeClimbSpeed, staticClimbSpeed * 0.4f);

                distanceJoint.distance -= verticalInput * activeClimbSpeed * Time.deltaTime;
                distanceJoint.distance = Mathf.Clamp(distanceJoint.distance, 1f, maxDistance);
            }

            if (Input.GetButtonDown("Jump"))
            {
                ReleaseHook(true); 
            }
        }
    }

    void FixedUpdate()
    {
        if (currentState == HookState.Hooked)
        {
            if (horizontalInput != 0)
            {
                Vector2 swingForceVector = new Vector2(horizontalInput * swingForce, 0);
                rb.AddForce(swingForceVector, ForceMode2D.Force);
            }

            if (rb.linearVelocity.magnitude > maxSpeedLimit)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeedLimit;
            }
        }
    }

    private IEnumerator ThrowHookRoutine()
    {
        currentState = HookState.Throwing;
        tensionStress = 0f; 
        
        Vector2 origin = GetOrigin();
        Vector2 direction = (currentMousePos - origin).normalized; 

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, hookableLayer);
        Vector2 targetPos = hit.collider != null ? hit.point : origin + direction * maxDistance;
        
        if (shootParticles != null) shootParticles.Play();
        if (audioSource != null && shootSFX != null) audioSource.PlayOneShot(shootSFX);

        lineRenderer.enabled = true;
        lineRenderer.startColor = normalColor;
        lineRenderer.endColor = normalColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        float distanceToTarget = Vector2.Distance(origin, targetPos);
        float currentDistance = 0f;

        while (currentDistance < distanceToTarget && currentState == HookState.Throwing)
        {
            currentDistance += hookThrowSpeed * Time.deltaTime;
            Vector2 currentTipPos = origin + direction * Mathf.Min(currentDistance, distanceToTarget);
            lineRenderer.SetPosition(0, GetOrigin());
            lineRenderer.SetPosition(1, currentTipPos);
            yield return null;
        }

        if (currentState != HookState.Throwing) yield break; 

        if (hit.collider != null)
        {
            currentState = HookState.Hooked;
            distanceJoint.connectedAnchor = hit.point;
            distanceJoint.distance = Vector2.Distance(GetOrigin(), hit.point); 
            distanceJoint.enabled = true;
            
            playerCollider.sharedMaterial = frictionlessMaterial;
            if (playerController != null) playerController.enabled = false;
        }
        else
        {
            currentState = HookState.Retracting;
            while (currentDistance > 0f && currentState == HookState.Retracting)
            {
                currentDistance -= hookReturnSpeed * Time.deltaTime; 
                Vector2 currentTipPos = origin + direction * Mathf.Max(currentDistance, 0f);
                lineRenderer.SetPosition(0, GetOrigin());
                lineRenderer.SetPosition(1, currentTipPos);
                yield return null;
            }
            currentState = HookState.Inactive;
            lineRenderer.enabled = false;
        }
    }

    void ReleaseHook(bool withJump, bool wasSnapped = false)
    {
        if (currentState == HookState.Inactive || currentState == HookState.Retracting) return;

        if (wasSnapped)
        {
            if (snapParticles != null)
            {
                ParticleSystem ps = Instantiate(snapParticles, GetOrigin(), Quaternion.identity);
                ps.transform.parent = transform; 
            }
            if (audioSource != null && snapSFX != null) audioSource.PlayOneShot(snapSFX);
        }

        currentState = HookState.Inactive;
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        tensionStress = 0f; 

        playerCollider.sharedMaterial = defaultMaterial;
        if (playerController != null) playerController.enabled = true;

        if (withJump)
        {
            Vector2 currentVelocity = rb.linearVelocity;
            bool isPerfectTiming = currentVelocity.magnitude > (maxSpeedLimit * 0.6f);
            
            if (isPerfectTiming)
            {
                float finalVelX = currentVelocity.x * horizontalPreservationMultiplier * perfectTimingMultiplier;
                float finalVelY = verticalPopForce * perfectTimingMultiplier;
                rb.linearVelocity = new Vector2(finalVelX, finalVelY);
            }
            else
            {
                float cleanY = currentVelocity.y < 0 ? 0 : currentVelocity.y;
                float finalX = currentVelocity.x;

                if (horizontalInput != 0)
                {
                    finalX = horizontalInput * Mathf.Max(Mathf.Abs(currentVelocity.x), normalJumpHorizontalControl);
                }

                rb.linearVelocity = new Vector2(finalX, cleanY + normalJumpPop);
            }
        }
    }
}