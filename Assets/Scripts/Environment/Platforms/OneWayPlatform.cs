using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private PlayerMove4 player; // 또는 너의 플레이어 스크립트
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMove4>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.onLadder && Input.GetAxisRaw("Vertical") < 0)
        {
            //effector.surfaceArc = 0;
            effector.rotationalOffset = 180;
        }
        else
        {
            //effector.surfaceArc = 170;
            effector.rotationalOffset = 0;
        }
        Debug.Log(effector.surfaceArc);

    }
/*
문제
: 각도를 바꿔서 애니메이션이 부자연스러움 
하지만 오르고 내리고 점프도 가능함..

*/
}
