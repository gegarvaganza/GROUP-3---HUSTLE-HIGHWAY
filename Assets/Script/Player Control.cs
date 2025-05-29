using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // === Jetpack Variables ===
    private bool jetpackEquip = false;
    public float mThrust = 30f;               // Jetpack thrust strength
    public GameObject jetpack;                // Jetpack visual object
    public float jetpackDuration = 5f;        // Duration in seconds
    private float jetpackTimer = 0f;          // Timer countdown
    // ==========================

    // === Explosion Powerup Variables ===
    private bool explosionPowerupEquip = false;
    public float explosionPowerupDuration = 5f;  // Explosion powerup duration in seconds
    private float explosionPowerupTimer = 0f;    // Timer countdown
    // ====================================

    // === Added for road section instantiation ===
    public GameObject roadSection;
    // ============================================

    void Start()
    {
        m_char = GetComponent<CharacterController>();
        ColHeight = m_char.height;
        ColCenterY = m_char.center.y;
        m_Animator = GetComponent<Animator>();
        transform.position = Vector3.zero;

        // === Jetpack Init ===
        jetpack.SetActive(false);
        // ====================
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

        // === Jetpack Thrust ===
        if (jetpackEquip && Input.GetButton("Fire1"))
        {
            y += mThrust * Time.deltaTime;
            if (y > JumpPower * 2f)
                y = JumpPower * 2f;
        }
        // ======================

        // === Jetpack Timer Countdown ===
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
        // ===============================

        // === Explosion Powerup Timer Countdown ===
        if (explosionPowerupEquip)
        {
            explosionPowerupTimer -= Time.deltaTime;
            if (explosionPowerupTimer <= 0f)
            {
                explosionPowerupEquip = false;
                explosionPowerupTimer = 0f;
                Debug.Log("Explosion powerup expired");
            }

            // Use mouse click (left button) to activate explosion effect
            if (Input.GetMouseButtonDown(0)) // 0 = left mouse button
            {
                ExplodeNearbyObstacles();
            }
        }
        // ==========================================

        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, 0);
        x = Mathf.Lerp(x, NewXPos, Time.deltaTime * SpeedDodge);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            Instantiate(roadSection, new Vector3(-202, -59, 162), Quaternion.identity);
        }

        // === Jetpack Powerup Pickup ===
        if (other.CompareTag("Powerup")) // Tag this to your jetpack powerup object
        {
            jetpackEquip = true;
            jetpack.SetActive(true);
            jetpackTimer = jetpackDuration;
            Destroy(other.gameObject);
        }
        // ===============================

        // === Explosion Powerup Pickup ===
        else if (other.CompareTag("ExplosionPowerup"))
        {
            explosionPowerupEquip = true;
            explosionPowerupTimer = explosionPowerupDuration;
            Destroy(other.gameObject);
        }
        // ===============================

        // Removed explosion effect on obstacle collision, now handled on mouse click
    }

    void ExplodeNearbyObstacles()
    {
        float explosionForce = 650f;
        float explosionRadius = 15.5f;
        float upwardsModifier = 1.5f;

        // Find all colliders within explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Obstacle"))
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                }
            }
        }

        Debug.Log("Explosion powerup activated!");
    }
}
