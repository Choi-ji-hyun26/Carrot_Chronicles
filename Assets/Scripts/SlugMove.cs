//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugMove : EnemyMove
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Invoke("Think", 5); // 5초뒤에 함수 호출
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove,rigid.velocity.y);
        
        //Platform Check
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVector, Vector3.down, new Color(0,1,0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 1, LayerMask.GetMask("Platform"));// 물리기반, 이 레이어에 해당하는 것만 스캔할 것
        if(rayHit.collider == null) 
        {
            Turn();
        }
    }

        protected virtual void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1,2); //최소값(포함), 최대값(미포함) -1: 왼쪽 0: idle, 1: 오른쪽

        // Sprite Animation
        animator.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if(nextMove != 0) //idle 상태에서는 방향전환이 필요 없으므로
            spriteRenderer.flipX = nextMove == 1; //true : 1 -> 체크된 거 오른쪽 방향

        //Recursive, 재귀 함수는 보통 함수 마지막에 위치!
        float nextThinkTime = Random.Range(2f,5f);
        Invoke("Think", nextThinkTime); //재귀 + 딜레이 
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke("Think");
        Invoke("Think",5);
    }
}
