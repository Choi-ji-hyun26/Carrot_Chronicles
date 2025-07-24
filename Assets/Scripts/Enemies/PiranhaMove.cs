using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaMove : EnemyBase
{
    public PiranhaAttack piranhaAttack;

    protected override void Awake()
    {
        base.Awake();
        if (piranhaAttack == null)
            piranhaAttack = GetComponent<PiranhaAttack>(); // fallback 안전장치
    }
    public void enbox()
    {
        piranhaAttack.enboxCollider();    
    }
    public void debox()
    {
        piranhaAttack.deboxCollider();
    }
}
