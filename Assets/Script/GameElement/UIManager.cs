using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using GUtility;

namespace GGame
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance => _instance;

        [SerializeField] private GameObject[] panelArray = new GameObject[Enum.GetNames(typeof(EPanel)).Length];
        [SerializeField] private Text stageClearText;
        [SerializeField] private Text gameScoreText;

        [SerializeField] private Transform scoreTitle;
        [SerializeField] private Transform timerTitle;
        [SerializeField] private Transform pivot; // 화면 아래 특정 타일을 기준으로 위치 대응

        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;

        private int basicScore = 0; // 이전 스테이지까지의 획득 스코어
        private int score; // 진행 중인 스테이지의 획득 스코어
        private int remainTime;
        
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
            scoreTitle.SafeSetPosition(new Vector2(scoreTitle.position.x, pivot.position.y));
            timerTitle.SafeSetPosition(new Vector2(timerTitle.position.x, pivot.position.y));
        }

        //시작 화면
        public void StartPanelSetActive(bool active)
        {
            panelArray[(int)EPanel.START].SafeSetActive(active);
            panelArray[(int)EPanel.KEY].SafeSetActive(!active);
            panelArray[(int)EPanel.UI].SafeSetActive(!active);
        }

        //UI 패널
        public void UIPanelActive()
        {
            panelArray[(int)EPanel.STAGE_CLEAR].SafeSetActive(false);
            panelArray[(int)EPanel.STAGE_FAIL].SafeSetActive(false);
            
            panelArray[(int)EPanel.KEY].SafeSetActive(true);
            panelArray[(int)EPanel.UI].SafeSetActive(true);
        }

        //스코어 갱신
        public void UpdateScore(int score)
        {
            this.score += score;
            
            scoreText.SafeSetText((basicScore + this.score).ToString());
        }

        //타이머 갱신
        public void UpdateTimer(int time)
        {
            timeText.SafeSetText(time.ToString());
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
                //DebugHelper.Log("Time Over");
                
                StageManager.Instance.StageResult(EResult.TIME_OVER);
            }

        }

        //결과 화면
        public void ResultPanelActive(EResult result)
        {
            StopAllCoroutines();
            
            panelArray[(int)EPanel.KEY].SafeSetActive(false);
            panelArray[(int)EPanel.UI].SafeSetActive(false);

            switch (result)
            {
                case EResult.GAME_OVER:
                {
                    panelArray[(int)EPanel.STAGE_FAIL].SafeSetActive(true);
                    score = 0;
                    UpdateScore(score);
                    break;
                }
                case EResult.TIME_OVER:
                {
                    panelArray[(int)EPanel.STAGE_FAIL].SafeSetActive(true);
                    score = 0;
                    UpdateScore(score);
                    break;
                }
                case EResult.STAGE_CLEAR:
                {
                    panelArray[(int)EPanel.STAGE_CLEAR].SafeSetActive(true);
                    
                    stageClearText.SafeSetText("STAGE CLEAR!");
                    
                    if (StageManager.Instance.GetCurrentStage < 3)
                    {
                        basicScore += score;
                        score = 0;
                        gameScoreText.SafeSetText("TAP TO NEXT STAGE");
                    }

                    break;
                }
                case EResult.GAME_CLEAR:
                {
                    panelArray[(int)EPanel.STAGE_CLEAR].SafeSetActive(true);
                    stageClearText.SafeSetText("GAME CLEAR!");
                    gameScoreText.SafeSetText((basicScore + score).ToString());
                    gameScoreText.SafeSetText($"SCORE {gameScoreText.text}");
                    break;
                }
            }
        }
    }
}