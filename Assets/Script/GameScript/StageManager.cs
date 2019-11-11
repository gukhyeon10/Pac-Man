using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance = null;

    [SerializeField]
    GameObject StartPagePanel;

    [SerializeField]
    ItemManager itemManager;

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

    const string dataPath = "MapData/";

    // 타일 29행 23열 (30행 타일은 가리기 용도)
    const int column = 23;
    const int line = 29;

    public Transform[,] tileArray = new Transform[line, column];
    public bool[,] movableCheckArray = new bool[line, column];

    int normalCount;  // 노말 아이템 총 개수

    public static StageManager Instance
    {
        get
        {
            return _instance;
        }
    }

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
        TapStartStage();
    }

    //시작 화면
    void TapStartStage()
    {
        StartPagePanel.SetActive(true);
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
                StartPagePanel.SetActive(false);

                InitTileArray();

                LoadStage(1);
                break;
            }
        }
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


        Debug.Log("Stage Load Success");

        InitCharacter(stageNumber);
    }

    // 각 캐릭터 초기화
    void InitCharacter(int stageNumber)
    {
        CharacterArray[(int)ECharacter.PAC].gameObject.SetActive(true);
        CharacterArray[(int)ECharacter.PAC].InitCharacter(2, 2);

        CharacterArray[(int)ECharacter.BLINKY].gameObject.SetActive(true);
        CharacterArray[(int)ECharacter.BLINKY].InitCharacter(2, 3);
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

    public void EatNormal()
    {
        normalCount--;
        if(normalCount<= 0)
        {
            Debug.Log("Game Clear!");
        }
    }

}
