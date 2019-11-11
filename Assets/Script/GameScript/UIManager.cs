using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;

    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    Transform scoreTitle;
    [SerializeField]
    Transform timerTitle;
    [SerializeField]
    Transform pivot; // 화면 아래 특정 타일을 기준으로 위치 대응

    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text timeText;

    int score;
    int remainTime;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    // UI 위치 초기화
    public void InitUI()
    {
        scoreTitle.position = new Vector2(scoreTitle.position.x, pivot.position.y);
        timerTitle.position = new Vector2(timerTitle.position.x, pivot.position.y);
    }

    //스코어 갱신
    public void UpdateScore(int score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
    }

    //타이머 갱신
    public void UpdateTimer(int time)
    {
        timeText.text = time.ToString();
    }

    //타이머 시작
    public void StartTimer(int time)
    {
        StartCoroutine(timeCheck(time));
    }

    //타이머 코루틴
    IEnumerator timeCheck(int time)
    {
        remainTime = time;
        UIManager.Instance.UpdateTimer(remainTime);
        while (remainTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainTime--;
            UpdateTimer(remainTime);
        }

        Debug.Log("Time Over");
    }

}
