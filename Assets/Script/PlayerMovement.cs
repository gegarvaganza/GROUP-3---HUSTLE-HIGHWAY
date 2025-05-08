using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool explosionPowerupEquip = false;
    private bool jetpackEquip = false;
    private bool dashPowerupEquip = false;
    private bool isDashing = false;
    private float dashTimer = 0f;

    public float forwardSpeed = 10f;
    public float laneDistance = 4f; // Distance between lanes (left, middle, right)
    public float jumpForce = 7f;
    public float gravity = -20f;
    public float mThrust = 30f; // Jetpack thrust (adjusted for better flying)
    public float dashSpeed = 50f; // Speed during dash
    public float dashDuration = 0.5f; // Dash movement time
    public GameObject jetpack;

    private CharacterController controller;
    private Vector3 moveDirection;
    private int currentLane = 1; // 0 = left, 1 = middle, 2 = right
    private float verticalVelocity;

    // New powerup timers
    private float jetpackTimer = 0f;
    private float explosionTimer = 0f;
    private float dashPowerupTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        jetpack.SetActive(false);
    }

    void Update()
    {
        // Forward movement
        moveDirection.z = isDashing ? dashSpeed : forwardSpeed;

        // Lane switching
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
            currentLane--;

        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
            currentLane++;

        // Jetpack flying
        if (jetpackEquip && Input.GetButton("Fire1"))
        {
            verticalVelocity += mThrust * Time.deltaTime;
            if (verticalVelocity > jumpForce * 2f)
                verticalVelocity = jumpForce * 2f;
        }

        // Dashing
        if (dashPowerupEquip && Input.GetKeyDown(KeyCode.F) && !isDashing)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                StopDash();
            }
        }

        // Target X position based on lane
        float targetX = (currentLane - 1) * laneDistance;
        float deltaX = targetX - transform.position.x;
        moveDirection.x = deltaX * 10f; // smoothing

        // Jumping and Gravity
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);

        // Explosion trigger by mouse click
        if (Input.GetMouseButtonDown(0) && explosionPowerupEquip)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 explosionPosition = hit.point;
                        float explosionForce = 500f;
                        float explosionRadius = 5f;
                        float upwardsModifier = 1.0f;

                        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                        Debug.Log("Obstacle exploded by click!");
                    }
                }
            }
        }

        // --- NEW: Update Powerup Timers ---

        if (jetpackEquip)
        {
            jetpackTimer -= Time.deltaTime;
            if (jetpackTimer <= 0f)
            {
                jetpackEquip = false;
                jetpack.SetActive(false);
                Debug.Log("Jetpack powerup ended.");
            }
        }

        if (explosionPowerupEquip)
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0f)
            {
                explosionPowerupEquip = false;
                Debug.Log("Explosion powerup ended.");
            }
        }

        if (dashPowerupEquip && !isDashing) // Only countdown if not dashing
        {
            dashPowerupTimer -= Time.deltaTime;
            if (dashPowerupTimer <= 0f)
            {
                dashPowerupEquip = false;
                Debug.Log("Dash powerup ended.");
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        Debug.Log("Started Dash!");
    }

    void StopDash()
    {
        isDashing = false;
        Debug.Log("Stopped Dash!");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDashing && hit.collider.CompareTag("Wall"))
        {
            Vector3 explosionPos = hit.point;
            float explosionForce = 500f;
            float explosionRadius = 5f;

            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach (Collider nearby in colliders)
            {
                if (nearby.CompareTag("Wall"))
                {
                    Rigidbody rb = nearby.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.constraints = RigidbodyConstraints.None;
                        rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 1f, ForceMode.Impulse);
                    }
                }
            }

            StopDash();
            Debug.Log("Wall Destroyed by Dash!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.name);

        if (other.CompareTag("Powerup")) // Jetpack Powerup
        {
            jetpackEquip = true;
            jetpackTimer = 3f; // 3 seconds
            jetpack.SetActive(true);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ExplosionPowerup")) // Explosion Powerup
        {
            explosionPowerupEquip = true;
            explosionTimer = 5f; // 5 seconds
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("DashPowerup")) // Dash Powerup
        {
            dashPowerupEquip = true;
            dashPowerupTimer = 2f; // 2 seconds
            Destroy(other.gameObject);
        }
    }
}
