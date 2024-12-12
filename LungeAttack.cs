using UnityEngine;

public class LungeAttack : MonoBehaviour
{
    public float lungeDistance = 5f;            // Distance the player moves during the lunge
    public float lungeDuration = 0.3f;          // Duration of the lunge in seconds
    public float lungeCooldown = 1.0f;          // Cooldown time between lunges in seconds
    public LayerMask enemyLayer;                // Layer mask for enemy detection

    public float attackHitboxRadius = 0.5f;     // Radius for detecting enemies
    public float gizmoSphereRadius = 0.5f;      // Gizmo sphere size for debugging

    private bool isLunging = false;
    private bool canLunge = true;               // Whether the player can lunge
    private Vector3 lungeStartPos;              // Starting position of the lunge
    private Vector3 lungeTargetPos;             // Target position of the lunge
    private float lungeTimer = 0f;

    public Transform orientation;               // Reference to the orientation Transform

    void Update()
    {
        // Start the lunge on left mouse button click if not lunging and cooldown is complete
        if (Input.GetMouseButtonDown(0) && canLunge && !isLunging)
        {
            StartLunge();
        }

        // Update lunge progress
        if (isLunging)
        {
            LungeMovement();
        }
    }

    void StartLunge()
    {
        isLunging = true;
        canLunge = false;
        lungeTimer = 0f;

        // Determine the lunge start and target positions
        lungeStartPos = transform.position;
        lungeTargetPos = transform.position + orientation.forward * lungeDistance;

        // Start cooldown timer
        Invoke(nameof(ResetLungeCooldown), lungeCooldown);
    }

    void LungeMovement()
    {
        lungeTimer += Time.deltaTime;

        // Calculate the interpolation factor (0 to 1)
        float interpolationFactor = lungeTimer / lungeDuration;

        // Smoothly move the player toward the target position
        transform.position = Vector3.Lerp(lungeStartPos, lungeTargetPos, interpolationFactor);

        // Check for enemies hit during the lunge
        CheckForEnemyHit();

        // End the lunge once the duration is complete
        if (lungeTimer >= lungeDuration)
        {
            EndLunge();
        }
    }

    void EndLunge()
    {
        isLunging = false;
    }

    void ResetLungeCooldown()
    {
        canLunge = true;
    }

    void CheckForEnemyHit()
    {
        // Check for enemy collisions during the lunge
        Vector3 detectionPoint = transform.position + orientation.forward * attackHitboxRadius;

        Collider[] hitEnemies = Physics.OverlapSphere(detectionPoint, attackHitboxRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(100);  // Adjust damage as needed
            }
            else
            {
                Destroy(enemy.gameObject);  // Instant kill if no health script
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the Gizmo for debugging purposes
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + orientation.forward * lungeDistance, gizmoSphereRadius);
    }
}
