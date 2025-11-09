using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GGame
{

    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance => _instance;

        [SerializeField] GameObject[] panelArray = new GameObject[Enum.GetNames(typeof(EPanel)).Length];
        [SerializeField] Text stageClearText;
        [SerializeField] Text gameScoreText;

        [SerializeField] Transform scoreTitle;
        [SerializeField] Transform timerTitle;
        [SerializeField] Transform pivot; // 화면 아래 특정 타일을 기준으로 위치 대응

        [SerializeField] Text scoreText;
        [SerializeField] Text timeText;

        int basicScore = 0; // 이전 스테이지까지의 획득 스코어
        int score; // 진행 중인 스테이지의 획득 스코어
        int remainTime;
        public bool isContinue = true;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // UI 위치 초기화
        public void InitUI()
        {
            scoreTitle.position = new Vector2(scoreTitle.position.x, pivot.position.y);
            timerTitle.position = new Vector2(timerTitle.position.x, pivot.position.y);
        }

        //시작 화면
        public void StartPanelSetActive(bool active)
        {
            panelArray[(int)EPanel.START].SetActive(active);
            panelArray[(int)EPanel.KEY].SetActive(!active);
            panelArray[(int)EPanel.UI].SetActive(!active);
        }

        //UI 패널
        public void UIPanelActive()
        {
            panelArray[(int)EPanel.STAGE_CLEAR].SetActive(false);
            panelArray[(int)EPanel.STAGE_FAIL].SetActive(false);
            panelArray[(int)EPanel.KEY].SetActive(true);
            panelArray[(int)EPanel.UI].SetActive(true);
        }

        //스코어 갱신
        public void UpdateScore(int score)
        {
            this.score += score;
            scoreText.text = (basicScore + this.score).ToString();
        }

        //타이머 갱신
        public void UpdateTimer(int time)
        {
            timeText.text = time.ToString();
        }

        //타이머 시작
        public void StartTimer(int time)
        {
            isContinue = true;
            StartCoroutine(TimeCheck(time));
        }

        WaitForSeconds waitForOneSeconds = new WaitForSeconds(1f);
        //타이머 코루틴
        IEnumerator TimeCheck(int time)
        {
            remainTime = time;
            UpdateTimer(remainTime);

            while (remainTime > 0)
            {
                if (!isContinue)
                {
                    break;
                }

                yield return waitForOneSeconds;

                remainTime--;
                UpdateTimer(remainTime);
            }

            if (isContinue)
            {
                Debug.Log("Time Over");
                StageManager.Instance.StageResult((int)EResult.TIME_OVER);

            }

        }

        //결과 화면
        public void ResultPanelActive(int result)
        {
            StopAllCoroutines();
            panelArray[(int)EPanel.KEY].SetActive(false);
            panelArray[(int)EPanel.UI].SetActive(false);

            switch (result)
            {
                case (int)EResult.GAME_OVER:
                {
                    panelArray[(int)EPanel.STAGE_FAIL].SetActive(true);
                    score = 0;
                    UpdateScore(score);
                    break;
                }
                case (int)EResult.TIME_OVER:
                {
                    panelArray[(int)EPanel.STAGE_FAIL].SetActive(true);
                    score = 0;
                    UpdateScore(score);
                    break;
                }
                case (int)EResult.STAGE_CLEAR:
                {
                    panelArray[(int)EPanel.STAGE_CLEAR].SetActive(true);
                    stageClearText.text = "STAGE CLEAR!";
                    if (StageManager.Instance.GetCurrentStage < 3)
                    {
                        basicScore += score;
                        score = 0;
                        gameScoreText.text = "TAP TO NEXT STAGE";
                    }

                    break;
                }
                case (int)EResult.GAME_CLEAR:
                {
                    panelArray[(int)EPanel.STAGE_CLEAR].SetActive(true);
                    stageClearText.text = "GAME CLEAR!";
                    gameScoreText.text = (basicScore + score).ToString();
                    gameScoreText.text = "SCORE " + gameScoreText.text;
                    break;
                }
            }
        }
    }
}