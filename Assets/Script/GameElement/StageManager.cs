using System.Collections;
using UnityEngine;
using System.Xml;
using System;
using GUtility;
using UnityEngine.UI;

namespace GGame
{
    public class StageManager : MonoBehaviour
    {
        // 타일 29행 23열 (30행 타일은 가리기 용도)
        public static int column = 23;
        public static int line = 29;
        
        private static StageManager _instance = null;
        public static StageManager Instance => _instance;
        
        [SerializeField] private Canvas gameCanvas;
        [SerializeField] private Text resultText;

        [SerializeField] private Transform tileGrid;

        // 각 캐릭터 (PAC = 0,  BLINKY = 1,  PINKY = 2,  INKY = 3,  CLYDE = 4,)
        [SerializeField] private CharacterBase[] CharacterArray = new CharacterBase[Enum.GetNames(typeof(ECharacter)).Length]; 
        
        public int GetCurrentStage => currentStage;
        
        public readonly TileModel tileModel = new TileModel();
        
        public readonly MoveModel moveModel = new MoveModel();
        
        public readonly GhostRespawnModel ghostRespawnModel = new GhostRespawnModel();

        private int currentStage = 1; // 현재 스테이지
        private int lastStage = 3; // 마지막 스테이지
        private int normalCount; // 노말 아이템 총 개수

        private bool isFirstTry = true; // 1스테이지 첫 시도인지

        private void Awake()
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

        private void Start()
        {
            //시작 화면
            UIManager.Instance.StartPanelSetActive(true);
            
            tileModel.Start(tileGrid);
            
            StartCoroutine(TapWaitCorutine());
        }

        //시작 화면 클릭 or 터치시 시작
        private IEnumerator TapWaitCorutine()
        {
            while (true)
            {
                yield return null;
                
                if (Input.GetKeyDown(KeyCode.Mouse0) || Input.touchCount > 0)
                {
                    if (isFirstTry) // 최초 스테이지 시작시에만
                    {
                        UIManager.Instance.InitUI();
                        
                        UIManager.Instance.StartPanelSetActive(false);
                        
                        isFirstTry = false;
                    }
                    else
                    {
                        UIManager.Instance.UIPanelActive();
                        
                        gameCanvas.SafeSetActive(true);
                    }
                    
                    InitStage();

                    LoadStage(currentStage);
                    
                    UIManager.Instance.StartTimer(40);

                    break;
                }
            }
        }

        //스테이지 초기화
        private void InitStage()
        {
            for (int i = 0; i < CharacterArray.Length; i++)
            {
                CharacterArray[i].isContinue = true;
            }
            
            tileModel.Init(tileGrid.childCount - column);
            
            moveModel.Init(tileGrid.childCount - column);
            
            ghostRespawnModel.Init(tileGrid.childCount - column);
            
            ItemManager.Instance.InitItem();
        }

        // 스테이지 로드
        private void LoadStage(int stageNumber)
        {
            var textAsset = (TextAsset)Resources.Load($"MapData/STAGE_{stageNumber.ToString()}");

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(textAsset.text);

            var nodeList = xmlDoc.SelectNodes("Map/Wall");
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    tileModel.SetUp(node);
                    
                    moveModel.SetUp(node);
                        
                    ghostRespawnModel.SetUp(node);
                }   
            }
            
            //아이템 로드
            nodeList = xmlDoc.SelectNodes("Map/Item");
            normalCount = ItemManager.Instance.LoadItem(nodeList);

            //캐릭터 위치 로드
            nodeList = xmlDoc.SelectNodes("Map/Character");
            InitCharacter(nodeList);
            //DebugHelper.Log("Stage Load Success");
        }

        // 각 캐릭터 초기화
        private void InitCharacter(XmlNodeList nodeList)
        {
            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    var row = node?.GetNode("Row")?.GetInt() ?? 0;
                    var col = node?.GetNode("Column")?.GetInt() ?? 0;
                    var number = node?.GetNode("Number")?.GetInt() ?? 0;

                    if (CharacterArray[number] != null)
                    {
                        CharacterArray[number].SafeSetActive(true);
                        CharacterArray[number].InitCharacter(row, col);
                    }
                }
            }
        }

        // 노말 아이템 획득 체크
        public void EatNormal()
        {
            normalCount--;
            
            if (normalCount <= 0)
            {
                //DebugHelper.Log("Game Clear!");
                StageResult(EResult.STAGE_CLEAR);
            }
        }

        //스테이지 초기화 및 비활성화 후 결과 화면 출력
        public void StageResult(EResult result)
        {
            switch (result)
            {
                case EResult.GAME_OVER:
                case EResult.TIME_OVER:
                {
                    StartCoroutine(StageFailEffect(result));
                    break;
                }
                case EResult.STAGE_CLEAR:
                case EResult.GAME_CLEAR:
                {
                    StartCoroutine(StageClearEffect());
                    break;
                }
            }
        }

        private WaitForSeconds waitForHalfSeconds = new WaitForSeconds(0.5f);
        //Stage Clear 연출
        private IEnumerator StageClearEffect()
        {
            //여기에 클리어 연출
            gameCanvas.gameObject.SetActive(false);

            if (currentStage >= lastStage)
            {
                //마지막 스테이지
                UIManager.Instance.ResultPanelActive(EResult.GAME_CLEAR);
                
                currentStage = 0;
            }
            else
            {
                UIManager.Instance.ResultPanelActive(EResult.STAGE_CLEAR);
            }

            //실수로 탭하여 바로 다음 스테이지 시작하지 않기 위해 잠깐의 텀
            yield return waitForHalfSeconds;
            
            currentStage++;
            
            CharacterBase.pac.InitPac();
            
            StartCoroutine(TapWaitCorutine());
        }

        private WaitForSeconds waitForTwoSeconds = new WaitForSeconds(2f);
        //Stage Fail 연출
        private IEnumerator StageFailEffect(EResult result)
        {
            for (int i = 0; i < CharacterArray.Length; i++)
            {
                CharacterArray[i].isContinue = false;
            }

            CharacterBase.pac.InitPac();

            UIManager.Instance.isContinue = false;

            //여기에 게임오버 연출
            yield return waitForTwoSeconds;
            
            StageFail(result);
            
            yield return waitForTwoSeconds;
            
            resultText.SafeSetActive(false);
            gameCanvas.SafeSetActive(false);
            
            UIManager.Instance.ResultPanelActive(result);

            //실수로 탭하여 바로 스테이지 시작하지 않기 위해 잠깐의 텀
            yield return waitForHalfSeconds;
            
            StartCoroutine(TapWaitCorutine());
        }

        public void StageFail(EResult result)
        {
            CharacterArray.SafeSetActive(false);

            if (result == EResult.GAME_OVER)
            {
                resultText.SafeSetText("GAME OVER");
            }
            else if (result == EResult.TIME_OVER)
            {
                resultText.SafeSetText("TIME OVER");
            }

            ItemManager.Instance.ItemPanelDisable();
            
            resultText.SafeSetActive(true);
            
            resultText.transform.SafeSetPosition(tileModel[ghostRespawnModel.RespawnCoord.row + 4, ghostRespawnModel.RespawnCoord.col].transform.position);
        }


        // 2차원 배열 방어 코드
        public static bool SafeArray<T>(T[,] array, (int row, int col) coord)
        {
            if (array != null && coord.row >= 0 && coord.row < line && coord.col >= 0 && coord.col < column && array[coord.row, coord.col] != null)
            {
                return true;
            }
            else
            {
                //Debug.Log(array.ToString() + " " + row.ToString() + "," + col.ToString() + "  is Array null");
                return false;
            }
        }
    }
}