using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
//using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/*
walk와 Jump 함수를 따로 만들었고
ladder 관련 함수를 새로 수정해본 코드
ladder 관련 큰 문제는 해결됐으나
점프가 제대로 안되고
사다리에서 점프가 안됨

*/
public class PlayerMove3 : MonoBehaviour
{
    public GameManager gameManager;
    public Transform groundCheck; // 발바닥 기준 위치
    public float maxSpeed = 6; //modi
    public Transform pos; //modi

    public float firstJumpForce = 20f; //modi
    public float doubleJumpForce = 10f;
    // 이단점프 위한 count 변수
    int jumpCount = 0;
    int maxJumpCount = 2;
    bool isJump; //modi
    public LayerMask isTile; //modi

    //new code about ladder
    public bool onLadder; //modi
    public float climbSpeed = 6;
    private float climbDirection;
    //private float gravityStore; //상황에 따라 중력을 다르게 하고 싶다면
    
    //지면에 있는지 체크
    private bool isGrounded; // 땅에 닿아있는지 //modi
    public float groundCheckRadius = 0.2f; //for isGrouded //modi
    public LayerMask groundLayer; //for isGrounded

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Animator animator;

    AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update() // 단발적인 Key 입력
    { 
        // // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 땅에 닿았으면 점프 카운트 초기화
        if (isGrounded)
        {
            jumpCount = 0;
        }
        
        // Jump
        if(Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {  
            isJump = true;
            Jump();
        }

        // 사다리 
        if(onLadder && Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0) //사다리와 겹치고 위아래 키 조작이 있어야만 
        {
            rigid.gravityScale = 0f;
            climbDirection = climbSpeed * Input.GetAxisRaw("Vertical");
            rigid.velocity = new Vector2(rigid.velocity.x, climbDirection);
            animator.SetBool("isClimbing", true); 
            animator.SetBool("isJumping",false);
            // 4.12 - 1 문제 위한 code
            // if(isGrounded)
            //     rigid.gravityScale = 4f;
            // Debug.Log(isGrounded + " " + onLadder);  // false true 나옴

        }
        else if(!onLadder)
        {
            rigid.gravityScale = 4f;
            animator.SetBool("isClimbing", false);
        }

        // Stop Speed
        if(Input.GetButtonUp("Horizontal")){
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        // Direction Sprite
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // Animation 
        if(Mathf.Abs(rigid.velocity.x) < 0.3) // 단위가 0이다 -> 멈춤
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        // 비탈길 중간에 멈출 시 미끄럼 방지
        if(Input.GetAxis("Horizontal") == 0) // idle 상태일때
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        else // 방향키 누를 때
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        
    }
    
    void FixedUpdate() // 지속적인 key 입력
    {
        /*
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.velocity.x > maxSpeed) //velocity: 리지드바디의 현 속도 // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //y 축 속도는 그대로 0을 넣어ㅡ면 안됨
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform
        if(rigid.velocity.y < 0){ //착지할때도 ray를 그리지 않게하기 위함
            Debug.DrawRay(groundCheck.position, Vector3.down * 0.3f, new Color(1,0,0));
            RaycastHit2D rayHit = Physics2D.Raycast(groundCheck.position, Vector3.down, 0.3f, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
            // rayHit : 빔을 쏘고 오브젝트에 대한 정보
            if(rayHit.collider != null) {// 빔을 맞았을 때
                if(rayHit.distance < 0.2f) //잘 밀착했는지
                    animator.SetBool("isJumping", false);
            }
        }
        */
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isTile);
        Walk();
        
        //Jump();

        if(onLadder)
        {
            float ver = Input.GetAxis("Vertical");
            rigid.gravityScale = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, ver * maxSpeed);
        }
        else
        {
            //Jump(); // onLadder가 false 일 때만 점프할 수 있게 -> 왱?
            rigid.gravityScale = 4f;
        }
        Debug.Log(isGrounded);
    }

    private void Walk()
    {
        float hor = Input.GetAxis("Horizontal");
        rigid.velocity = new Vector2(hor * maxSpeed, rigid.velocity.y);

        if(hor > 0)
        {
            transform.eulerAngles = new Vector3(0,0,0);
            animator.SetBool("isWalking", true);
        }
        else if(hor < 0)
        {
            transform.eulerAngles = new Vector3(0,180,0);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //isJump = true;
            rigid.AddForce(Vector2.up * firstJumpForce, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }
        else
        {
            //isJump = false;
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y < -0.01f && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            } else {
                OnDamaged(collision.transform.position);
            }
        }
    }

   private void OnTriggerEnter2D(Collider2D collision) {
        // Item : Carrot, Star
        if (collision.gameObject.tag == "Item"){
            //health
            bool isCarrot = collision.gameObject.name.Contains("Carrot");
            if(isCarrot)
                gameManager.HealthUp();
            // point
            bool isStar = collision.gameObject.name.Contains("Star");
            if(isStar)
                gameManager.stagePoint += 1;
            //Deactive Item
            collision.gameObject.SetActive(false);

            // Sound
            //PlaySound("ITEM");

        }
        // 포탈 : 피니시 구간
        if (collision.gameObject.tag == "Finish"){ 
            //Next Stage
            gameManager.NextStage();

            //Sound
            //PlaySound("FINISH"); 
        }
        if(collision.gameObject.tag == "Ladders")
        {
            onLadder = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.tag == "Ladders")
        {
            onLadder = false;
        }
    }
    void OnAttack(Transform enemy)
    {
        //point
        //gameManager.stagePoint += 100;
        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        //Sound
        //PlaySound("ATTACK"); 
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        //Change Layer (Immortal Active)
        gameObject.layer = 11; // PlayerDamaged의 Layer 번호
        spriteRenderer.color = new Color(1,1,1,0.4f); //0.4 : 투명도

        //Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //왼쪽에서 맞으면 왼쪽으로, 오른쪽에서 맞으면 오른쪽으로 튕기기
        rigid.AddForce(new Vector2(dirc,1) * 10,ForceMode2D.Impulse);
    
        // Animation
        animator.SetTrigger("doDamaged");

        //Sound
        //PlaySound("DAMAGED");

        Invoke("OffDamaged", 3); // 무적시간 3초
    }

    void OffDamaged()
    {
        gameObject.layer = 10; //Player의 Layer 번호
        spriteRenderer.color = new Color(1,1,1,1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1,1,1,0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        boxCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Sound
        //PlaySound("DIE"); 
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
