using UnityEngine;

public class AiMovement : MonoBehaviour
{
    public PlayerControl player; // Reference to the player
    private Animator aiAnimator;

    private SIDE lastPlayerSide;
    private bool lastInJump;
    private bool lastInCrouch;

    // Movement speed settings
    private float followSpeed;
    public float initialFollowSpeed = 10f;
    public float reducedFollowSpeed = 6f;
    public float speedReduceDelay = 3f;
    private float speedTimer = 0f;

    void Start()
    {
        aiAnimator = GetComponent<Animator>();

        if (player == null)
        {
            Debug.LogError("AIAnimator: Player reference not assigned!");
            return;
        }

        lastPlayerSide = player.m_SIDE;
        lastInJump = player.inJump;
        lastInCrouch = player.inCrouch;

        followSpeed = initialFollowSpeed; // Start with fast follow speed
    }

    void Update()
    {
        if (player == null) return;

        // Gradually slow down after delay
        if (speedTimer < speedReduceDelay)
        {
            speedTimer += Time.deltaTime;
        }
        else
        {
            followSpeed = reducedFollowSpeed;
        }

        // Move AI towards player's X and Y (keep Z fixed)
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // SIDE movement animation
        if (player.m_SIDE != lastPlayerSide)
        {
            switch (player.m_SIDE)
            {
                case SIDE.Left:
                    aiAnimator.CrossFadeInFixedTime("Left2", 0.1f);
                    break;
                case SIDE.Right:
                    aiAnimator.CrossFadeInFixedTime("Right2", 0.1f);
                    break;
                case SIDE.Mid:
                    // Optional idle or running animation
                    break;
            }
            lastPlayerSide = player.m_SIDE;
        }

        // Jump animation
        if (player.inJump && !lastInJump)
        {
            aiAnimator.CrossFadeInFixedTime("Jump2", 0.1f);
        }
        lastInJump = player.inJump;

        // Crouch animation
        if (player.inCrouch && !lastInCrouch)
        {
            aiAnimator.CrossFadeInFixedTime("Crouch2", 0.1f);
        }
        lastInCrouch = player.inCrouch;

        // Falling / Landing animation
        AnimatorStateInfo playerState = player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (playerState.IsName("Falling2"))
        {
            aiAnimator.CrossFadeInFixedTime("Falling2", 0.1f);
        }
        else if (playerState.IsName("Landing2"))
        {
            aiAnimator.CrossFadeInFixedTime("Landing2", 0.1f);
        }
    }
}