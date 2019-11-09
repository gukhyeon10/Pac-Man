using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class StageManager : MonoBehaviour
{
    private static StageManager _instance = null;

    [SerializeField]
    GameObject StartPagePanel;

    [SerializeField]
    Transform tileGrid;

    [SerializeField]
    CharacterBase[] CharacterArray = new CharacterBase[5]; // 각 캐릭터 (PAC = 0,  BLINKY = 1,  PINKY = 2,  INKY = 3,  CLYDE = 4,)

    const string dataPath = "MapData/";

    // 타일 29행 23열 (30행 타일은 가리기 용도)
    const int column = 23;  
    const int line = 29;    
    
    public Transform[ , ] tileArray = new Transform[line, column];
    public bool[ , ] movableCheckArray = new bool[line, column];

    public static StageManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if(_instance == null)
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
        while(true)
        {
            yield return null;
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.touchCount > 0)
            {
                StartPagePanel.SetActive(false);

                InitTileArray();

                InitStage(2);
                break;
            }
        }
    }


    // 타일 리스트, 이동 가능 체크
    void InitTileArray()
    {  
        for (int i=0; i<tileGrid.childCount - column; i++)   // 마지막 행 타일들은 유령을 가리기위한 타일이기에 게임에 영향X
        {
            tileArray[(i / column), (i % column)] = tileGrid.GetChild(i);
            movableCheckArray[(i / column), (i % column)] = false;
        }
        
    }

    // 스테이지 로드
    void InitStage(int stageNumber)
    {
       
        string stageFileName = "STAGE_TEST_";
        stageFileName += stageNumber.ToString();

        TextAsset textAsset = (TextAsset)Resources.Load(dataPath + stageFileName);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Tile");

        string tileName = string.Empty;
        
        //행과 열은 1,1부터 시작
        int row = 1, col = 1;
        foreach (XmlNode node in nodeList)
        {
            tileName = node.SelectSingleNode("Name").InnerText;

            tileArray[row, col].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("TileSprite/" + tileName, typeof(Sprite));

            tileArray[row, col].eulerAngles = new Vector3(0, 0, float.Parse(node.SelectSingleNode("Rot").InnerText));

            if (tileName.Equals("Default_Sprite") == false)   // 빈 타일이 아니면 이동 불가
            {
                movableCheckArray[row, col] = false;
            }
            else
            {
                movableCheckArray[row, col] = true;      // 빈 타일이면 이동 가능

                //화면상 테두리부분이 뚫려있다면 테두리 한칸 밖 타일 이동가능
                if(row == 1)                             
                {
                    movableCheckArray[row - 1, col] = true;
                }
                if(row == line-2)                        
                {
                    movableCheckArray[row + 1, col] = true;
                }
                if(col == 1)                             
                {
                    movableCheckArray[row, col - 1] = true;
                }
                if(col == column -2)                     
                {
                    movableCheckArray[row, col + 1] = true;
                }
            }


            col++;
            if(col > column-2)   
            { 
                //타일 한 행 로드시 다음 행 시작
                row++;
                col = 1;
            }

        }

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


}
