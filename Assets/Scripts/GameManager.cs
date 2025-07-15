using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    public PlayerMove4 player;

    public int health = 3;
    public GameObject[] Stages;

    public UnityEngine.UI.Image UIHealth;
    public Sprite hpSprite3;
    public Sprite hpSprite2;
    public Sprite hpSprite1;
    public Sprite hpSprite0;

    public Text UIPoint;
    public Text UIStage;
    public Text feverText;
    public GameObject UIRestartBtn;
    public GameObject miniMapCamera;
    public GameObject SettingMenu;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject); // 이미 존재하면 중복 방지
        }
    }
    void Update()
    {
        if (UIPoint != null) //UI Point가 사용되는 씬에서만 실행되도록
            UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //if(miniMapCamera.SetActive(true))
        miniMapCamera.SetActive(false);
        //Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1); //stageIndex는 0부터 시작해서 +1
        }
        else
        { // Game Clear
            //Player Control Lock
            Time.timeScale = 0;

            //Restart Button UI
            //Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            //btnText.text = "Clear!";
            ViewBtn();

            //Ending Scene
            SceneManager.LoadScene("Ending");
        }
        //Calculation Poing
        totalPoint += stagePoint;
        stagePoint = 0;
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
            player.OnDie();
            //Invoke("ViewBtn",2); // 게임매니저가 2초 사이에 파괴된다면 문제 발생
            ViewBtn();
        }
    }
    // 무적 타임, playermove에서 호출
    public void ActivateFeverMode()
    {
        StartCoroutine(FeverCoroutine());
    }
    private IEnumerator FeverCoroutine()
    {
        int feverDuration = 7;

        feverText.gameObject.SetActive(true);

        for (int i = feverDuration; i > 0; i--)
        {
            feverText.text = $"FEVER {i}";
            yield return new WaitForSeconds(1f);
        }

        feverText.text = "";
        feverText.gameObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (health > 1) // 마지막 체력에서 낭떨어지일때는 원위치 하지 않기
            {
                PlayerReposition();
            }
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-6, -1, -10);

        player.VelocityZero();
    }

    void ViewBtn()
    {
        UIRestartBtn.SetActive(true);
    }

    public void Restart()
    {
        /*
        restart 후 별 제대로 카운트 안 된 이유
        GameManager가 DontDestroyOnLoad 상태로 유지되면서 
        재시작해도 내부 값이 초기화되지 않고 남아 있기 때문
        아래의 164번 코드를 추가하여 해결!
         */
        Destroy(GameManager.Instance.gameObject); // 씬 이동 전에 호출 -> restart 후에도 Score 제대로 카운트됨 
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void ViewSettingMenu()
    {
        SettingMenu.SetActive(true);
    }
    public void Resume()
    {
        SettingMenu.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit(); //유니티 프로그램 내에선 Application.Quit() 함수가 작동하지 않는 것이 정상
    }
}
