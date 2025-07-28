using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChestHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public bool isUnBeatTime = false; // public : Player Attack/Damaged/Chest Handler 호출,무적 타임
    public bool isClover = false; // public : ChestItem에서 이 값을 설정

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HandleChest(GameObject chestObject) // public : PlayerTriggerHandler 호출
    {
        if (!isClover) return;

        gameObject.layer = LayerMask.NameToLayer("InvinciblePlayer");

        isUnBeatTime = true;
        StartCoroutine(UnBeatTime());
        GetComponent<FeverUIController>()?.ActivateFeverMode();
        SoundManager.Instance.PlaySound("ITEM");
    }

    private IEnumerator UnBeatTime()
    {
        int countTime = 0;

        while (countTime < 35) // 35 * 0.2 = 7초
        {
            if (countTime % 2 == 0)
                spriteRenderer.color = new Color32(255, 255, 255, 90); //반투명
            else
                spriteRenderer.color = new Color32(255, 255, 255, 180); // 좀더 진하게

            yield return new WaitForSeconds(0.2f); // 깜빡임, 35 * 0.2 = 7초

            countTime++;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = new Color32(255, 255, 255, 255);
        isUnBeatTime = false;
        isClover = false;

        yield return null;
    }
}
