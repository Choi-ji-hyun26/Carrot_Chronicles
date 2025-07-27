using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaMove : EnemyBase
{
    [SerializeField] private PiranhaAttack piranhaAttack; // 인스펙터 드래그 허용 -> SerializeField , 외부접근 막기 -> private

    protected override void Awake()
    {
        base.Awake();
        if (piranhaAttack == null)
            piranhaAttack = GetComponent<PiranhaAttack>(); // fallback 안전장치
    }
    public void enbox() // 유니티 엔진 애니메이션에서 호출
    {
        piranhaAttack.enboxCollider();    
    }
    public void debox() // 유니티 엔진 애니메이션에서 호출
    {
        piranhaAttack.deboxCollider();
    }
}
