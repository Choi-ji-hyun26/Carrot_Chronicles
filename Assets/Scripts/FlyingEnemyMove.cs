using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyMove : EnemyMove
{
    public float speed = 1f; 
    public float moveDistance = 2f; //이동 범위
    private Vector3 startPos;
    bool isDead = false;


    protected override void Awake()
    {
        base.Awake();
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        if (isDead) return; // 죽었을 때 fixedupdate() 계속 실행되지 않도록

        float offset = Mathf.Sin(Time.time * speed) * moveDistance;
        Vector2 newPos = new Vector2(transform.position.x, startPos.y + offset);
        rigid.MovePosition(newPos);
    }

    public override void OnDamaged()
    {
        isDead = true;
        rigid.gravityScale = 4f;
        base.OnDamaged();
    }
}
