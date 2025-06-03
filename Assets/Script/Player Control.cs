using UnityEngine;
using System.Collections;

[System.Serializable]
public enum SIDE { Left, Mid, Right }

public class PlayerControl : MonoBehaviour
{
    public SIDE m_SIDE = SIDE.Mid;
    float NewXPos = 0f;
    [HideInInspector]
    public bool SwipeLeft, SwipeRight, SwipeUp, SwipeDown;
    public float XValue;
    private CharacterController m_char;
    private Animator m_Animator;
    private float x;
    public float SpeedDodge;
    public float JumpPower = 7f;
    private float y;
    public bool inJump;
    public bool inCrouch;
    private float ColHeight;
    private float ColCenterY;

    // Jetpack variables
    private bool jetpackEquip = false;
    public float mThrust = 30f;
    public GameObject jetpack;
    public float jetpackDuration = 5f;
    private float jetpackTimer = 0f;

    // Explosion powerup variables
    private bool explosionPowerupEquip = false;
    public float explosionPowerupDuration = 5f;
    private float explosionPowerupTimer = 0f;

    // Invincibility variables
    private bool hasInvincibilityPowerup = false;
    private bool isInvincible = false;
    public float invincibilityDuration = 5f;
    private float invincibilityTimer = 0f;

    private Renderer playerRenderer;
    private Collider playerCollider;

    void Start()
    {
        m_char = GetComponent<CharacterController>();
        ColHeight = m_char.height;
        ColCenterY = m_char.center.y;
        m_Animator = GetComponent<Animator>();
        transform.position = Vector3.zero;
        jetpack.SetActive(false);

        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
            Debug.LogWarning("Player Renderer not found! Invincibility color effect won't work.");

        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
            Debug.LogWarning("Player Collider not found!");
    }

    void Update()
    {
        // Input detection
        SwipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        SwipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        SwipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        SwipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

        // Side movement logic
        if (SwipeLeft)
        {
            if (m_SIDE == SIDE.Mid)
            {
                NewXPos = -XValue;
                m_SIDE = SIDE.Left;
                m_Animator.Play("Left");
            }
            else if (m_SIDE == SIDE.Right)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("Left");
            }
        }
        else if (SwipeRight)
        {
            if (m_SIDE == SIDE.Mid)
            {
                NewXPos = XValue;
                m_SIDE = SIDE.Right;
                m_Animator.Play("Right");
            }
            else if (m_SIDE == SIDE.Left)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("Right");
            }
        }

        // Jetpack controls
        if (jetpackEquip && Input.GetButton("Fire1"))
        {
            y += mThrust * Time.deltaTime;
            if (y > JumpPower * 2f)
                y = JumpPower * 2f;
        }

        // Timers for powerups
        if (jetpackEquip)
        {
            jetpackTimer -= Time.deltaTime;
            if (jetpackTimer <= 0f)
            {
                jetpackEquip = false;
                jetpack.SetActive(false);
                jetpackTimer = 0f;
            }
        }

        if (explosionPowerupEquip)
        {
            explosionPowerupTimer -= Time.deltaTime;
            if (explosionPowerupTimer <= 0f)
            {
                explosionPowerupEquip = false;
                explosionPowerupTimer = 0f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ExplodeNearbyObstacles();
            }
        }

        // --- INVINCIBILITY ACTIVATION ---

        // Reduce timer if powerup available
        if (hasInvincibilityPowerup)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                // Powerup expired
                hasInvincibilityPowerup = false;
                DeactivateInvincibility();
            }
            else
            {
                // While holding F activate invincibility
                if (Input.GetKey(KeyCode.F))
                {
                    if (!isInvincible)
                        ActivateInvincibility();
                }
                else
                {
                    if (isInvincible)
                        DeactivateInvincibility();
                }
            }
        }

        // Movement calculations (Z-axis movement removed!)
        x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
        Vector3 moveVector = new Vector3(
            x - transform.position.x,
            y * Time.deltaTime,
            0f // No forward (Z) movement
        );
        m_char.Move(moveVector);

        Jump();
        Crouch();
    }

    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = Mathf.Max(invincibilityTimer, invincibilityDuration); // Reset timer if needed
        hasInvincibilityPowerup = true;

        if (playerRenderer != null)
            playerRenderer.material.color = Color.yellow;

        SetCollisionWithObstacles(false); // Disable collision with obstacles

        Debug.Log("Invincibility Activated");
    }

    private void DeactivateInvincibility()
    {
        isInvincible = false;
        if (playerRenderer != null)
            playerRenderer.material.color = Color.white;

        SetCollisionWithObstacles(true); // Re-enable collisions

        Debug.Log("Invincibility Deactivated");
    }

    void SetCollisionWithObstacles(bool enabled)
    {
        Collider[] allColliders = Object.FindObjectsByType<Collider>(FindObjectsSortMode.None);
        foreach (var col in allColliders)
        {
            if (col.CompareTag("Obstacle") && playerCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, col, !enabled);
            }
        }
    }

    public void Jump()
    {
        if (m_char.isGrounded)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                m_Animator.Play("Landing");
                inJump = false;
            }

            if (SwipeUp)
            {
                y = JumpPower;
                m_Animator.CrossFadeInFixedTime("Jump", 0.1f);
                inJump = true;
            }
            else
            {
                y = -1f;
            }
        }
        else
        {
            y -= JumpPower * 2 * Time.deltaTime;

            if (m_char.velocity.y < -0.1f && !inCrouch)
            {
                m_Animator.Play("Falling");
            }
        }
    }

    internal float CrouchCounter;
    public void Crouch()
    {
        CrouchCounter -= Time.deltaTime;
        if (CrouchCounter <= 0f)
        {
            CrouchCounter = 0f;
            m_char.center = new Vector3(0, ColCenterY, 0);
            m_char.height = ColHeight;
            inCrouch = false;
        }

        if (SwipeDown)
        {
            CrouchCounter = 0.2f;
            y -= 10f;
            m_char.center = new Vector3(0, ColCenterY / 2f, 0);
            m_char.height = ColHeight / 2f;
            m_Animator.CrossFadeInFixedTime("Crouch", 0.1f);
            inCrouch = true;
            inJump = false;
        }
    }

    void ExplodeNearbyObstacles()
    {
        float explosionForce = 650f;
        float explosionRadius = 15.5f;
        float upwardsModifier = 1.5f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Obstacle"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                    StartCoroutine(ReEnableKinematic(rb, 0.5f));
                }
            }
        }
        Debug.Log("Explosion powerup activated!");
    }

    IEnumerator ReEnableKinematic(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            Rigidbody obstacleRb = hit.collider.GetComponent<Rigidbody>();
            if (obstacleRb != null && obstacleRb.isKinematic == false)
            {
                obstacleRb.isKinematic = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            jetpackEquip = true;
            jetpack.SetActive(true);
            jetpackTimer = jetpackDuration;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ExplosionPowerup"))
        {
            explosionPowerupEquip = true;
            explosionPowerupTimer = explosionPowerupDuration;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("InvincibilityPowerup"))
        {
            hasInvincibilityPowerup = true;
            invincibilityTimer = invincibilityDuration;
            Debug.Log("Invincibility powerup picked up! Hold F to activate.");
            Destroy(other.gameObject);
        }
    }
}
