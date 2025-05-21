using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -4) * Time.deltaTime;
    }
}
