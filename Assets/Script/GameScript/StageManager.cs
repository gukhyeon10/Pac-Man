using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using UnityEngine.UI;

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
    Text resultText;

    [SerializeField]
    Transform tileGrid;

    [SerializeField]
    SpriteManager spriteManager;

    [SerializeField]
    CharacterBase[] CharacterArray = new CharacterBase[Enum.GetNames(typeof(ECharacter)).Length]; // 각 캐릭터 (PAC = 0,  BLINKY = 1,  PINKY = 2,  INKY = 3,  CLYDE = 4,)

    [SerializeField]
    ItemManager itemManager;
    UIManager _UIManager;

    const string dataPath = "MapData/";
    const string stageFileName = "STAGE_TEST_";

    // 타일 29행 23열 (30행 타일은 가리기 용도)
    const int column = 23;
    const int line = 29;

    public GameTile[,] tileArray = new GameTile[line, column];
    public bool[,] movableCheckArray = new bool[line, column];
    public bool[,] ghostRespawnMovableCheckArray = new bool[line, column];
    public int ghostRespawnRow = line/2, ghostRespawnCol = column/2;  //유령 디폴트 초기 위치

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
        _UIManager = UIManager.Instance;
        //시작 화면
        _UIManager.StartPanelSetActive(true);

        InitTileArray();
        InitMovableArray();
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
                if (isFirstTry)  // 최초 스테이지 시작시에만
                {
                    _UIManager.InitUI();
                    _UIManager.StartPanelSetActive(false);
                    isFirstTry = false;
                }
                else
                {

                    _UIManager.UIPanelActive();
                    gameCanvas.gameObject.SetActive(true);
                    InitStage();

                }

                LoadStage(currentStage);
                _UIManager.StartTimer(40);

                break;
            }
        }
    }

    //스테이지 초기화
    void InitStage()
    {
        Sprite defaultSprite = tileArray[0, 0].spriteRenderer.sprite;
        for (int row = 0; row < line; row++)
        {
            for (int col = 0; col < column; col++)
            {
                if(SafeArray(tileArray, row, col))
                {
                    tileArray[row, col].spriteRenderer.sprite = defaultSprite;
                }
            }
        }

        for(int i=0; i<CharacterArray.Length; i++)
        {
            CharacterArray[i].isContinue = true;
        }
        
        InitMovableArray();
        itemManager.InitItem();
    }


    // 타일 리스트, 이동 가능 체크
    void InitTileArray()
    {
        for (int i = 0; i < tileGrid.childCount - column; i++)   // 마지막 행 타일들은 유령을 가리기위한 타일이기에 게임에 영향X
        {
            tileArray[(i / column), (i % column)] = tileGrid.GetChild(i).GetComponent<GameTile>();
        }

        itemManager.tileArray = this.tileArray;
    }

    // 이동가능 체크 초기화
    void InitMovableArray()
    {
        for (int i = 0; i < tileGrid.childCount - column; i++)   // 마지막 행 타일들은 유령을 가리기위한 타일이기에 게임에 영향X
        {
            if (i / column > 0 && i / column < line - 1 && i % column > 0 && i % column < column - 1)   // 화면상 타일들은 이동가능이 디폴트
            {
                if (SafeArray<bool>(movableCheckArray, i / column, i % column))
                {
                    movableCheckArray[(i / column), (i % column)] = true;
                    ghostRespawnMovableCheckArray[(i / column), (i % column)] = true;
                }

            }
            else                                                                         // 화면밖 테두리 타일들은 이동불가능이 디폴트
            {
                if (SafeArray<bool>(movableCheckArray, i / column, i % column))
                {
                    movableCheckArray[(i / column), (i % column)] = false;
                    ghostRespawnMovableCheckArray[(i / column), (i % column)] = true;
                }

            }
        }
    }

    // 스테이지 로드
    void LoadStage(int stageNumber)
    {
        string stage = stageFileName;
        stage += stageNumber.ToString();

        TextAsset textAsset = (TextAsset)Resources.Load(dataPath + stage);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        int row, col, objectNumber;
        XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Wall");

        foreach (XmlNode node in nodeList)
        {
            row = int.Parse(node.SelectSingleNode("Row").InnerText);
            col = int.Parse(node.SelectSingleNode("Column").InnerText);
            objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);


            tileArray[row, col].spriteRenderer.sprite = spriteManager.wallSpriteArray[objectNumber];
            tileArray[row, col].transform.eulerAngles = new Vector3(0f, 0f, float.Parse(node.SelectSingleNode("Rot").InnerText));

            movableCheckArray[row, col] = false;
            ghostRespawnMovableCheckArray[row, col] = false;
            if (objectNumber == (int)EWall.CENTERDOOR)
            {
                ghostRespawnMovableCheckArray[row, col] = true;
                ghostRespawnRow = row;
                ghostRespawnCol = col;
            }
        }


        //화면상 테두리부분이 뚫려있다면 테두리 한칸 밖 타일 이동가능
        for (int x = 1; x < column - 1; x++)
        {
            if (SafeArray<GameTile>(tileArray, 1, x) && tileArray[1, x].spriteRenderer.sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[0, x] = true;
            }
            if (SafeArray<GameTile>(tileArray, line -2, x) && tileArray[line - 2, x].spriteRenderer.sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[line - 1, x] = true;
            }
        }
        for (int y = 1; y < line - 1; y++)
        {
            if (SafeArray<GameTile>(tileArray, y, 1) && tileArray[y, 1].spriteRenderer.sprite.name.Equals("Default_Sprite"))
            {
                movableCheckArray[y, 0] = true;
            }
            if (SafeArray<GameTile>(tileArray, y, column-2) && tileArray[y, column - 2].spriteRenderer.sprite.name.Equals("Default_Sprite"))
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

            if (CharacterArray[characterNumber] != null)
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
        switch (result)
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
        _UIManager.ResultPanelActive((int)EResult.STAGE_CLEAR);

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
        for(int i=0; i<CharacterArray.Length; i++)
        {
            CharacterArray[i].isContinue = false;
        }
        CharacterBase.pac.InitPac();

        UIManager.Instance.isContinue = false;

        //여기에 게임오버 연출
        yield return new WaitForSeconds(2f);
        StageFail(result);
        yield return new WaitForSeconds(2f);
        resultText.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        _UIManager.ResultPanelActive(result);

        yield return new WaitForSeconds(1f);
        StartCoroutine(TapWaitCorutine());
    }

    public void StageFail(int result)
    {
        for(int i = 0; i<CharacterArray.Length; i++)
        {
            CharacterArray[i].gameObject.SetActive(false);
        }
        
        if(result == (int)EResult.GAME_OVER)
        {
            resultText.text = "GAME OVER";
        }
        else if(result == (int)EResult.TIME_OVER)
        {
            resultText.text = "TIME OVER";
        }

        itemManager.ItemPanelDisable();
        resultText.gameObject.SetActive(true);
        resultText.transform.position = tileArray[ghostRespawnRow + 4, ghostRespawnCol].transform.position;

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


    // 2차원 배열 방어 코드
    public static bool SafeArray<T>(T[,] array, int row, int col)
    {
        if(array != null && row>=0 && row <=line && col>=0 && col<=column && array[row, col] != null)
        {
            return true;
        }
        else
        {
            Debug.Log(array.ToString() + " " + row.ToString() + "," + col.ToString() + "  is Array null");
            return false;
        }
    }
}