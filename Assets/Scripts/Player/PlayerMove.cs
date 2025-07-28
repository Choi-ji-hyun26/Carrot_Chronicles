using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    // public : Player 관련 스크립트
    public Rigidbody2D rigid { get; private set; } 
    public SpriteRenderer spriteRenderer { get; private set; }
    public BoxCollider2D boxCollider { get; private set; }
    public Animator animator { get; private set; }

    [SerializeField] private float maxSpeed = 6;

    // jump
    [SerializeField] private float firstJumpForce = 20f;
    [SerializeField] private float doubleJumpForce = 16f;
    private int jumpCount = 0; // 이단점프 위한 count 변수
    private int maxJumpCount = 2;
    [SerializeField] private bool isJumping = false;

    // ladder
    public bool onLadder; // public : Ladder.cs
    [SerializeField] private float climbSpeed = 6;
    private float climbDirection;
    //private float gravityStore; //상황에 따라 중력을 다르게 하고 싶다면
    [SerializeField] private bool isClimbing = false;

    //isGronded : 사다리, 점프 // RayCast
    [SerializeField] private bool isGrounded = false; // 땅에 닿아있는지, 현재 프레임의 접지 상태
    [SerializeField] private ContactFilter2D groundFilter; // Layer : Platform
    [SerializeField] private float groundCheckDistance = 0.1f;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5]; // 결과 저장용 배열

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update() // 단발적인 Key 입력
    {
        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        // Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation 
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // 단위가 0이다 -> 멈춤
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        // 비탈길 중간에 멈출 시 미끄럼 방지
        if (Input.GetAxis("Horizontal") == 0) // idle 상태일때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else // 방향키 누를 때
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (isGrounded) //5.23 사다리 다 내려갔음에도 오르는 애니메이션 유지되길래 추가함 -> 해결됨
        {
            animator.SetBool("isClimbing", false);
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumpCount)
            {
                isJumping = true;
                animator.SetBool("isJumping", true);
                animator.SetBool("isClimbing", false);
            }
            SoundManager.Instance.PlaySound("JUMP");
        }

        // 사다리
        if (onLadder && Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f) //사다리와 겹치고 위아래 키 조작이 있어야만 
        {
            isClimbing = true;

            animator.SetBool("isClimbing", true);
            animator.SetBool("isJumping", false); // 사다리에서 점프로 오르기 할 때 애니메이션 변경위해 

        }
        else if (!onLadder)
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
        }
    }

    private void FixedUpdate() // 지속적인 key 입력
    {
        //Landing Platform
        CheckGrounded();

        // 땅에 닿았으면 점프 카운트 초기화
        if (isGrounded)
        {
            jumpCount = 0;
        }

        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //velocity: 리지드바디의 현 속도 // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //y 축 속도는 그대로 0을 넣어ㅡ면 안됨
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        

        //사다리
        if (isClimbing)
        {
            rigid.gravityScale = 0f;

            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
            {
                climbDirection = climbSpeed * Input.GetAxisRaw("Vertical");
                rigid.velocity = new Vector2(rigid.velocity.x, climbDirection);
            }
            else
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0); // 움직임 없음
            }

            Debug.Log("Gravity : " + rigid.gravityScale);
        }
        else
        {
            rigid.gravityScale = 4f;
            Debug.Log("Gravity : " + rigid.gravityScale);
        }

        //JUMP
        if (isJumping)
        {
            float appliedForce = (jumpCount == 0) ? firstJumpForce : doubleJumpForce; // 점프 횟수에 따라 높이 다르게 적용

            //수직 속도 초기화해서 점프 높이 일정하게
            if (jumpCount > 0)
                rigid.velocity = new Vector2(rigid.velocity.x, 0);

            rigid.velocity = new Vector2(rigid.velocity.x, appliedForce);
            jumpCount++;
            isJumping = false;

            Debug.Log($"Jump Count: {jumpCount}, Applied Force: {appliedForce}");

        }
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        if (rigid.velocity.y < 0) // 낙하 중일 때만 검사
        {
            int hitCount = rigid.Cast(Vector2.down, groundFilter, groundHits, groundCheckDistance);

            if (hitCount > 0)
            {
                if (groundHits[0].distance < groundCheckDistance)
                {
                    isGrounded = true;
                    animator.SetBool("isJumping", false);
                }
            }
        }
    }
    public void VelocityZero() // public : GameManager 호출
    {
        rigid.velocity = Vector2.zero;
    }
}