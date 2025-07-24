using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaMove : EnemyBase
{
    public PiranhaAttack piranhaAttack;

    public override void OnDamaged()
    {
        // isDead = true;
        base.OnDamaged();
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
