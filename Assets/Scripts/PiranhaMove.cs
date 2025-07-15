using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaMove : EnemyMove
{
    bool isDead = false;

    protected override void Awake()
    {
        base.Awake();

        rigid.constraints = RigidbodyConstraints2D.FreezePosition  | RigidbodyConstraints2D.FreezeRotation;  //움직임 고정, x,y 모두    
        rigid.gravityScale = 0f; //중력 사용 안 함
    }
    void FixedUpdate()
    {
        if (isDead) return; // 죽었을 때 fixedupdate() 계속 실행되지 않도록
    }
    public override void OnDamaged()
    {
        isDead = true;
        rigid.gravityScale = 4f;
        base.OnDamaged();
    }
    // public void enbox()
    // {
    //     piranhaAttack.enboxCollider();    
    // }
    // public void debox()
    // {
    //     piranhaAttack.deboxCollider();
    // }
}
