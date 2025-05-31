using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour
{
    public float initialSpeed = 4f;      // Starting speed
    public float maxSpeed = 20f;        // Maximum allowed speed
    public float acceleration = 0.1f;   // How quickly speed increases
    private float currentSpeed;         // Current speed (starts at initialSpeed)

    void Start()
    {
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        // Increase speed, but never exceed maxSpeed
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        
        // Move the road
        transform.position += new Vector3(0, 0, -currentSpeed) * Time.deltaTime;
    }
}
