using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public static Stats instance;

    public int stagePoint;

    public int health = 3;

    public UnityEngine.UI.Image UIHealth;
    public Sprite hpSprite3;
    public Sprite hpSprite2;
    public Sprite hpSprite1;
    public Sprite hpSprite0;

    public Text UIPoint;
    void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Debug.LogWarning("씬에 두개 이상의 스탯 매니저가 존재합니다!");
            Destroy(gameObject); // 이미 존재하면 중복 방지
        }
    }


    void Update()
    {
        if (UIPoint != null) //UI Point가 사용되는 씬에서만 실행되도록
            UIPoint.text = stagePoint.ToString();
    }

    public void HealthUp()
    {
        if (health < 3)
            health++;
        if (health == 2)
            UIHealth.sprite = hpSprite2;
        else
            UIHealth.sprite = hpSprite3;
    }
    public void HealthDown()
    {
        health--;
        if (health == 2)
        {
            UIHealth.sprite = hpSprite2;
        }
        else if (health == 1)
        {
            UIHealth.sprite = hpSprite1;
        }
        else if (health == 0)
        {
            UIHealth.sprite = hpSprite0;
            gameObject.GetComponentInParent<PlayerDeathHandler>().OnDie();
            //Invoke("ViewBtn",2); // 게임매니저가 2초 사이에 파괴된다면 문제 발생
            GameManager.Instance.ViewBtn();
        }
    }
}
