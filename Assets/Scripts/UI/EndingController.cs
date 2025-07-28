using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    private void Start()
    {
        int score = Stats.instance.stagePoint;
        scoreText.text = "SCORE " + score.ToString();
    }

    public void OnRestartButtonClick() // public : 유니티 UI BUTTON 연결
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager is null!");
            return;
        }
        GameManager.Instance.Restart();
    }
}
