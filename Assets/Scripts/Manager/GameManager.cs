using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    [SerializeField] private int stageIndex;

    [SerializeField] private PlayerMove player;

    [SerializeField] private GameObject[] Stages;

    [SerializeField] private Text UIStage;

    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject UIRestartBtn;
    [SerializeField] private GameObject miniMapCamera;
    [SerializeField] private GameObject SettingMenu;

    private void Awake()
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

    public void NextStage() // public : PlayerTriggerHandler에서 호출
    {
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
        else // Game Clear
        {
            //Player Control Lock
            Time.timeScale = 0;

            ViewBtn();

            //Ending Scene 로드
            LoadScenes();
        }
    }

    private void LoadScenes()
    {
        if (Stats.instance.stagePoint > 15) // 진 엔딩
            SceneManager.LoadScene("TrueEnding");
        else // 일반엔딩
            SceneManager.LoadScene("NormalEnding");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Stats.instance.health > 1) // 마지막 체력에서 낭떨어지일때는 원위치 하지 않기
            {
                PlayerReposition();
            }
            Stats.instance.HealthDown();
        }
    }

    private void PlayerReposition()
    {
        player.transform.position = new Vector3(-6, -1, -10);

        player.VelocityZero();
    }

    // 플레이어 상태 Die 인 경우
    public void ViewBtn() // public : 유니티 엔진 UI BUTTON 연결
    {
        darkOverlay.SetActive(true);
        UIRestartBtn.SetActive(true);
    }

    public void Restart() // public : EndingController에서 호출
    {
        /*
        restart 후 별 제대로 카운트 안 된 이유
        GameManager가 DontDestroyOnLoad 상태로 유지되면서 
        재시작해도 내부 값이 초기화되지 않고 남아 있기 때문
        아래의 두개의 Destroy 코드를 추가하면서 해결!
         */
        Destroy(GameManager.Instance.gameObject); // 씬 이동 전에 호출 -> restart 후에도 Score 제대로 카운트됨 
        Destroy(Stats.instance.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void ViewSettingMenu() // public : 유니티 엔진 UI BUTTON 연결, 설정
    {
        SettingMenu.SetActive(true);
    }
    public void Resume() // public : 유니티 엔진 UI BUTTON 연결, 설정 - 계속하기
    {
        SettingMenu.SetActive(false);
    }
    public void Exit() // public : 유니티 엔진 UI BUTTON 연결 ,설정 - 나가기
    {
        Application.Quit();
    }
}
