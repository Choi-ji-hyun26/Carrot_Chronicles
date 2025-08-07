using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // 외부 접근 차단 + 인스펙터 표시
    [SerializeField] private Transform player; // 캐릭터 Transform 
    [SerializeField] private float smooth = 5f; // 부드러운 카메라 이동 속도
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10); // 카메라 오프셋

    private bool isFollowing = true; // 플레이어를 따라가는지

    private void LateUpdate()
    {
        if (player != null && isFollowing) 
            transform.position = Vector3.Lerp(transform.position, player.position + offset, smooth * Time.deltaTime);
    }

    public void StopFollowing()
    {
        isFollowing = false;
    }
}
