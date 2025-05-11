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
    [HideInInspector]
    public bool SwipeLeft, SwipeRight, SwipeUp;
    public float XValue;
    private CharacterController m_char;
    private Animator m_Animator;
    private float x;
    public float SpeedDodge;
    public float JumpPower = 7f;
    private float y;
    public bool InJump;
    public bool InRoll;
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
        SwipeUp = Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

        if (SwipeLeft)
        {
            if(m_SIDE == SIDE.Mid)
            {
                NewXPos = -XValue;
                m_SIDE = SIDE.Left;
                m_Animator.Play("Left");
            }
            else if(m_SIDE == SIDE.Right)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("Left");
            }
        }
        else if (SwipeRight)
        {
            if(m_SIDE == SIDE.Mid)
            {
                NewXPos = XValue;
                m_SIDE = SIDE.Right;
                m_Animator.Play("Right");
            }
            else if(m_SIDE == SIDE.Left)
            {
                NewXPos = 0;
                m_SIDE = SIDE.Mid;
                m_Animator.Play("Right");
            }
        }
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, 0);
        x = Mathf.Lerp(x,NewXPos,Time.deltaTime * SpeedDodge);
        m_char.Move(moveVector);
        Jump();
    }

    public void Jump()
    {
        if(m_char.isGrounded)
        {
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Falling"))
            {
                m_Animator.Play("Landing");
                InJump = false;
            }
            if (SwipeUp)
            {
                y = JumpPower;
                m_Animator.CrossFadeInFixedTime("Jump", 0.1f);
                InJump = true;
            }
            else
            {
                y -= JumpPower * 2 * Time.deltaTime;
                if(m_char.velocity.y<-0.1f)
                m_Animator.Play("Falling");
            }
        }
    }
}
