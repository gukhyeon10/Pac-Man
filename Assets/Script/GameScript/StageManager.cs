using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance = null;


    public static StageManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    Canvas gameCanvas;

    [SerializeField]
    Transform tileGrid;

    [SerializeField]
    Sprite[] wallSpriteArray = new Sprite[Enum.GetNames(typeof(EWall)).Length];
    [SerializeField]
    Sprite[] itemSpriteArray = new Sprite[Enum.GetNames(typeof(EItem)).Length];
    [SerializeField]
    Sprite[] characterSpriteArray = new Sprite[Enum.GetNames(typeof(ECharacter)).Length];

    [SerializeField]
    CharacterBase[] CharacterArray = new CharacterBase[Enum.GetNames(typeof(ECharacter)).Length]; // 각 캐릭터 (PAC = 0,  BLINKY = 1,  PINKY = 2,  INKY = 3,  CLYDE = 4,)

    [SerializeField]
    ItemManager itemManager;

    const string dataPath = "MapData/";
    // 타일 29행 23열 (30행 타일은 가리기 용도)
    const int column = 23;
    const int line = 29;

    public Transform[,] tileArray = new Transform[line, column];
    public bool[,] movableCheckArray = new bool[line, column];

    int currentStage = 1;  // 현재 스테이지
    int normalCount;  // 노말 아이템 총 개수

    bool isFirstTry = true;  // 1스테이지 첫 시도인지
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

    void Start()
    {
        //시작 화면
        UIManager.Instance.StartPanelSetActive(true);
        StartCoroutine(TapWaitCorutine());
    }

    //시작 화면 클릭 or 터치시 시작
    IEnumerator TapWaitCorutine()
    {
        while (true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.touchCount > 0)
            {
                if(isFirstTry)  // 최초 스테이지 시작시에만
                {
                    InitTileArray();
                    UIManager.Instance.InitUI();
                    UIManager.Instance.StartPanelSetActive(false);
                    isFirstTry = false;
                }
                else
                {
                    UIManager.Instance.UIPanelActive();
                    gameCanvas.gameObject.SetActive(true);
                    InitStage();
                    
                }

                LoadStage(currentStage);
                UIManager.Instance.StartTimer(50);

                break;
            }
        }
    }

    //스테이지 초기화
    void InitStage()
    {

        InitTileArray();

        Sprite defaultSprite = tileArray[0, 0].GetComponent<SpriteRenderer>().sprite;
        for(int row = 0; row < line; row++)
        {
            for(int col = 0; col < column; col++)
            {
                tileArray[row, col].GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
        }

        itemManager.InitItem();
    }


    // 타일 리스트, 이동 가능 체크
    void InitTileArray()
    {
        for (int i = 0; i < tileGrid.childCount - column; i++)   // 마지막 행 타일들은 유령을 가리기위한 타일이기에 게임에 영향X
        {
            tileArray[(i / column), (i % column)] = tileGrid.GetChild(i);
            if (i / column > 0 && i / column < line - 1 && i % column > 0 && i % column < column - 1)   // 화면상 타일들은 이동가능이 디폴트
            {
                movableCheckArray[(i / column), (i % column)] = true;
            }
            else                                                                         // 화면밖 테두리 타일들은 이동불가능이 디폴트
            {
                movableCheckArray[(i / column), (i % column)] = false;
            }
        }

        itemManager.tileArray = this.tileArray;

    }
    
    // 스테이지 로드
    void LoadStage(int stageNumber)
    {
        string stageFileName = "STAGE_TEST_";
        stageFileName += stageNumber.ToString();

        TextAsset textAsset = (TextAsset)Resources.Load(dataPath + stageFileName);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        int row, col, objectNumber;
        XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Wall");

        foreach (XmlNode node in nodeList)
        {
            row = int.Parse(node.SelectSingleNode("Row").InnerText);
            col = int.Parse(node.SelectSingleNode("Column").InnerText);
            objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);

            tileArray[row, col].GetComponent<SpriteRenderer>().sprite = wallSpriteArray[objectNumber];
            tileArray[row, col].eulerAngles = new Vector3(0f, 0f, float.Parse(node.SelectSingleNode("Rot").InnerText));

            movableCheckArray[row, col] = false;
        }


        //화면상 테두리부분이 뚫려있다면 테두리 한칸 밖 타일 이동가능
        for (int x = 1; x < column - 1; x++)
        {
            if (tileArray[1, x].GetComponent<SpriteRenderer>().sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[0, x] = true;
            }
            if (tileArray[line - 2, x].GetComponent<SpriteRenderer>().sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[line - 1, x] = true;
            }
        }
        for (int y = 1; y < line - 1; y++)
        {
            if (tileArray[y, 1].GetComponent<SpriteRenderer>().sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[y, 0] = true;
            }
            if (tileArray[y, column - 2].GetComponent<SpriteRenderer>().sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[y, column - 1] = true;
            }
        }

        //아이템 로드
        nodeList = xmlDoc.SelectNodes("Map/Item");
        normalCount = itemManager.LoadItem(nodeList);

        //캐릭터 위치 로드
        nodeList = xmlDoc.SelectNodes("Map/Character");
        InitCharacter(nodeList);
        Debug.Log("Stage Load Success");

    }

    // 각 캐릭터 초기화
    void InitCharacter(XmlNodeList nodeList)
    {
        int row, col, characterNumber;
        foreach (XmlNode node in nodeList)
        {
            row = int.Parse(node.SelectSingleNode("Row").InnerText);
            col = int.Parse(node.SelectSingleNode("Column").InnerText);
            characterNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
            
            if(CharacterArray[characterNumber] != null)
            {
                CharacterArray[characterNumber].gameObject.SetActive(true);
                CharacterArray[characterNumber].InitCharacter(row, col);
            }
        }
    }

    // 노말 아이템 획득 체크
    public void EatNormal()
    {
        normalCount--;
        if (normalCount <= 0)
        {
            Debug.Log("Game Clear!");
            StageResult((int)EResult.STAGE_CLEAR);

        }
    }

    //스테이지 초기화 및 비활성화 후 결과 화면 출력
    public void StageResult(int result)
    {
        switch(result)
        {
            case (int)EResult.GAME_OVER:
                {
                    StartCoroutine(StageFailEffect(result));
                    break;
                }
            case (int)EResult.TIME_OVER:
                {
                    StartCoroutine(StageFailEffect(result));
                    break;
                }
            case (int)EResult.STAGE_CLEAR:
                {
                    StartCoroutine(StageClearEffect());
                    break;
                }
            case (int)EResult.GAME_CLEAR:
                {
                    StartCoroutine(StageClearEffect());
                    break;
                }
        }
    }

    //Stage Clear 연출
    IEnumerator StageClearEffect()
    {
        //여기에 클리어 연출
        gameCanvas.gameObject.SetActive(false);
        UIManager.Instance.ResultPanelActive((int)EResult.STAGE_CLEAR);

        if (currentStage < 3)
        {
            yield return new WaitForSeconds(1f);
            currentStage++;
            StartCoroutine(TapWaitCorutine());
        }
    }

    //Stage Fail 연출
    IEnumerator StageFailEffect(int result)
    {
        //여기에 게임오버 연출
        yield return null;
        gameCanvas.gameObject.SetActive(false);
        UIManager.Instance.ResultPanelActive(result);

        yield return new WaitForSeconds(1f);
        StartCoroutine(TapWaitCorutine());
    }


    public int GetLine
    {
        get
        {
            return line;
        }
    }

    public int GetColumn
    {
        get
        {
            return column;
        }
    }

    public int GetCurrentStage
    {
        get
        {
            return this.currentStage;
        }
    }

}
