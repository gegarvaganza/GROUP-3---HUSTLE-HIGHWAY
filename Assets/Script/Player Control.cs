using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public enum SIDE { Left,Mid,Right }
public class PlayerControl : MonoBehaviour
{
    public SIDE m_SIDE = SIDE.Mid; 
    float NewXPos = 0f;
    public bool SwipeLeft;
    public bool SwipeRight;
    public float XValue;
    private CharacterController m_char;
    private Animator m_Animator;
    private float x;
    public float SpeedDodge;
    void Start()
    {
        m_char = GetComponent<CharacterController>();
        transform.position = Vector3.zero;
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SwipeLeft = Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        SwipeRight = Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        if (SwipeLeft)
        {
            if(m_SIDE == SIDE.Mid)
            {
                NewXPos = -XValue;
                m_SIDE = SIDE.Left;
                m_Animator.Play("TurnLeft");
            }
            else if(m_SIDE == SIDE.Right)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("TurnLeft");
            }
        }
        else if (SwipeRight)
        {
            if(m_SIDE == SIDE.Mid)
            {
                NewXPos = XValue;
                m_SIDE = SIDE.Right;
                m_Animator.Play("TurnRight");
            }
            else if(m_SIDE == SIDE.Left)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("TurnRight");
            }
        }
        x = Mathf.Lerp(x,NewXPos,Time.deltaTime * SpeedDodge);
        m_char.Move((x - transform.position.x) * Vector3.right);
    }
}
