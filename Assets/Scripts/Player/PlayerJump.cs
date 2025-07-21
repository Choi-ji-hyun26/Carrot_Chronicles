using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rigid;
    Animator animator;
    // code about jump
    public float firstJumpForce = 20f;
    public float doubleJumpForce = 16f;
    int jumpCount = 0; // 이단점프 위한 count 변수
    int maxJumpCount = 2;
    public bool isJumping = false;

    public bool isGrounded = false; // 땅에 닿아있는지
    public float groundCheckRadius = 0.2f; //for isGrouded
    public LayerMask groundLayer; //for isGrounded
    public Transform groundCheck; // 발바닥 기준 위치

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
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
    }

    void FixedUpdate() // 지속적인 key 입력
    {
        // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 땅에 닿았으면 점프 카운트 초기화
        if (isGrounded)
        {
            jumpCount = 0;
        }

        //Landing Platform
        if (rigid.velocity.y < 0)
        { //착지할때도 ray를 그리지 않게하기 위함
            Debug.DrawRay(groundCheck.position, Vector3.down * 0.3f, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.3f, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
            // rayHit : 빔을 쏘고 오브젝트에 대한 정보
            if (rayHit.collider != null)
            {// 빔을 맞았을 때
                if (rayHit.distance < 0.2f) //잘 밀착했는지
                    animator.SetBool("isJumping", false);
            }
        }

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
}
