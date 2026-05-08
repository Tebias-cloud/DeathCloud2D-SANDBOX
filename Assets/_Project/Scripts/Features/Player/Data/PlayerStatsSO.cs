using UnityEngine;

namespace DeathCloud.Player
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "DeathCloud/Player/Stats")]
    public class PlayerStatsSO : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 10f;
        public float jumpForce = 16f;

        [Header("Wall Mechanics")]
        public float wallSlidingSpeed = 1.5f;
        public Vector2 wallJumpPower = new Vector2(12f, 18f);
        public float wallJumpDuration = 0.2f;

        [Header("Grappling Hook")]
        public float maxHookDistance = 20f;
        public float swingForce = 40f;
        public float maxSpeedLimit = 25f;
        public float hookThrowSpeed = 90f;
        public float hookReturnSpeed = 140f;
        public float snappingTimeSlack = 12f;
        public float snappingTimeTense = 5f;

        [Header("Spider-Man Jump")]
        public float normalJumpPop = 16f;
        public float verticalPopForce = 15f;
        public float perfectTimingMultiplier = 1.6f;

        [Header("Detection")]
        public float groundCheckRadius = 0.3f;
        public float wallCheckRadius = 0.35f;
        public LayerMask groundLayer;
        public LayerMask hookableLayer;

        [Header("Juiciness & Game Feel")]
        public float coyoteTime = 0.15f;
        public float jumpBufferTime = 0.15f;

        [Header("Dash")]
        public float dashSpeed = 25f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 0.8f;

        [Header("Combat")]
        public float attackRange = 1.5f;
        public int attackDamage = 10;
        public LayerMask damageableLayer;
    }
}
