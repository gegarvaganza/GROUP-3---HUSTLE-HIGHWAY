using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

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

    // Audio
    public AudioClip jumpSfx;
    public AudioClip crouchSfx;
    public AudioClip moveLeftSfx;
    public AudioClip moveRightSfx;
    private AudioSource audioSource;

    // Jetpack variables
    private bool jetpackEquip = false;
    public float mThrust = 30f;
    public GameObject jetpack;
    public float jetpackDuration = 5f;
    private float jetpackTimer = 0f;

    // Explosion powerup
    private bool explosionPowerupEquip = false;
    public float explosionPowerupDuration = 5f;
    private float explosionPowerupTimer = 0f;

    // Wall explosion powerup
    private bool wallExplosionPowerupEquip = false;
    public float wallExplosionPowerupDuration = 5f;
    private float wallExplosionPowerupTimer = 0f;
    private bool wallExplosionReady = false;

    private Renderer playerRenderer;
    private Collider playerCollider;

    // Health system
    private float playerHealth = 100f;
    private float maxHealth = 100f;
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public PowerupUI powerupUI;
    public GameObject gameOverPanel;

    // Collider hit cooldown
    private float hitCooldown = 0f;

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
            Debug.LogWarning("Player Renderer not found! Effects may not show.");

        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
            Debug.LogWarning("Player Collider not found!");

        // Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateHealthUI();
    }

    void Update()
    {
        SwipeLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        SwipeRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        SwipeUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        SwipeDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

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
            if (moveLeftSfx != null) audioSource.PlayOneShot(moveLeftSfx, 0.9f);
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
            if (moveRightSfx != null) audioSource.PlayOneShot(moveRightSfx, 0.9f);
        }

        if (jetpackEquip && Input.GetButton("Fire1"))
        {
            y += mThrust * Time.deltaTime;
            if (y > JumpPower * 2f)
                y = JumpPower * 2f;
        }

        if (jetpackEquip)
        {
            jetpackTimer -= Time.deltaTime;
            if (jetpackTimer <= 0f)
            {
                jetpackEquip = false;
                jetpack.SetActive(false);
                if (powerupUI != null)
                    powerupUI.RemoveJetpack();
            }
        }

        if (explosionPowerupEquip)
        {
            explosionPowerupTimer -= Time.deltaTime;
            if (explosionPowerupTimer <= 0f)
            {
                explosionPowerupEquip = false;
                if (powerupUI != null)
                    powerupUI.RemoveExplosion();
            }
            if (Input.GetMouseButtonDown(0))
            {
                ExplodeNearbyObstacles();
            }
        }

        if (wallExplosionPowerupEquip)
        {
            wallExplosionPowerupTimer -= Time.deltaTime;
            if (wallExplosionPowerupTimer <= 0f)
            {
                wallExplosionPowerupEquip = false;
                if (powerupUI != null)
                    powerupUI.RemoveWallExplosion();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                ExplodeNearbyWalls();
            }
        }

        if (hitCooldown > 0f)
            hitCooldown -= Time.deltaTime;

        x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
        Vector3 moveVector = new Vector3(
            x - transform.position.x,
            y * Time.deltaTime,
            0f
        );
        m_char.Move(moveVector);

        Jump();
        Crouch();
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
                if (jumpSfx != null) audioSource.PlayOneShot(jumpSfx, 0.9f);
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
            if (crouchSfx != null) audioSource.PlayOneShot(crouchSfx, 0.9f);
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
        Debug.Log("Manual explosion activated!");
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
        if (hitCooldown > 0f) return;

        if (wallExplosionPowerupEquip && wallExplosionReady && hit.gameObject.CompareTag("Wall"))
        {
            // wall explosion logic
        }

        if (hit.gameObject.CompareTag("ObstacleTrigger"))
        {
            ReduceHealthByPercentage(0.3f);
            hitCooldown = 0.5f;
        }
    }

    void ExplodeNearbyWalls()
    {
        float explosionForce = 70f;
        float explosionRadius = 5f;
        float upwardsModifier = 0.2f;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Wall"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                    StartCoroutine(DestroyAfterDelay(hitCollider.gameObject, 1f));
                }
            }
        }

        Debug.Log("Wall explosion powerup used!");
    }

    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Destroy(obj);
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

            if (powerupUI != null)
                powerupUI.AddJetpack();
        }
        else if (other.CompareTag("ExplosionPowerup"))
        {
            explosionPowerupEquip = true;
            explosionPowerupTimer = explosionPowerupDuration;
            Destroy(other.gameObject);

            if (powerupUI != null)
                powerupUI.AddExplosion();
        }
        else if (other.CompareTag("WallExplosionPowerup"))
        {
            wallExplosionPowerupEquip = true;
            wallExplosionPowerupTimer = wallExplosionPowerupDuration;
            Destroy(other.gameObject);
            Debug.Log("Wall explosion powerup picked up!");

            if (powerupUI != null)
                powerupUI.AddWallExplosion();
        }
    }

    public void ReduceHealthByPercentage(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        float damage = maxHealth * percentage;
        playerHealth -= damage;
        playerHealth = Mathf.Clamp(playerHealth, 0f, maxHealth);
        Debug.Log($"Took damage: {damage}, Health now: {playerHealth}");
        UpdateHealthUI();

        if (playerHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = Mathf.Clamp01(playerHealth / maxHealth);
            float percent = (playerHealth / maxHealth) * 100f;
            healthText.text = Mathf.RoundToInt(percent) + "%";
        }
    }

    void Die()
    {
        Debug.Log("Player died at health: " + playerHealth);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
