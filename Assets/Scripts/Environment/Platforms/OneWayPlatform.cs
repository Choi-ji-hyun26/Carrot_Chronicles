using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    private PlayerMove player; // 또는 너의 플레이어 스크립트
    private void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
    }

    private void Update()
    {
        if (player.onLadder && Input.GetAxisRaw("Vertical") < 0)
        {
            effector.rotationalOffset = 180;
        }
        else
        {
            effector.rotationalOffset = 0;
        }
        Debug.Log(effector.surfaceArc);
    }
/*
아쉬운 점 : 애니메이션
해결 점 : 기본 오르내리기, 점프가능
*/
}
