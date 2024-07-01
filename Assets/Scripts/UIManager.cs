using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text coinText;
    public Text stageText;
    public Text timeText;
    private float remainingTime = 400f;
    private bool isTimeStopped = false; 

    void Update() {
        scoreText.text = GameManager.Instance.score.ToString("D6");
        coinText.text = "x"+GameManager.Instance.coinScore.ToString("D2");
        stageText.text = "1-1";
        // remainingTime 감소를 isTimeStopped 플래그로 제어
        if (remainingTime > 0 && !isTimeStopped) {
            remainingTime -= Time.deltaTime;
        }
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (remainingTime > 0) {
            timeText.text = Mathf.CeilToInt(remainingTime).ToString("D3");
        }
        else {
            timeText.text = "000";
        }
    }

    // 외부에서 호출 가능한 메서드로 시간 멈춤을 제어
    public void StopTime()
    {
        isTimeStopped = true;
    }

    public void StartTime()
    {
        remainingTime = 400f;
        isTimeStopped = false;
    }
}
