using UnityEngine;

public enum HitSide { Front, Left, Right }

public class ObstacleHitCollider : MonoBehaviour
{
    public HitSide side;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.ReduceHealthByPercentage(0.3f); // 30% of 100 = 30 damage
                Debug.Log($"Player hit obstacle from {side} side. Health reduced.");
            }
        }
    }
}