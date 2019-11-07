using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    Transform tileGrid;

    const string dataPath = "MapData/";
    const int columnCount = 21;    // 한 행에 21개 타일

    List<Transform> tileList;
    Dictionary<int, bool> dicMovable;

    [SerializeField]
    Transform pac;

    Transform target;

    // 위치 좌표 (초기값)
    int locationX = 1;  
    int locationY = 1;

    int moveDirect = 0;  // 0 left   1 right   2 up   3 down
    int inputDirect = 1;

    float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        InitTileList();

        InitStage();
    }

    void Update()
    {
        // 팩맨 이동
        pac.position = Vector3.MoveTowards(pac.position, target.position, speed * Time.deltaTime);
        
        if (pac.position == target.position)
        {
            switch(inputDirect)
            {
                case 0:
                    {
                        if(dicMovable[locationX * columnCount + locationY - 1])
                        {
                            target = tileList[locationX * columnCount + locationY - 1];
                            locationY--;
                        }
                        break;
                    }
                case 1:
                    {
                        if (dicMovable[locationX * columnCount + locationY +1])
                        {
                            target = tileList[locationX * columnCount + locationY + 1];
                            locationY++;
                        }
                        break;
                    }
                case 2:
                    {
                        if (dicMovable[(locationX-1) * columnCount + locationY])
                        {
                            target = tileList[(locationX-1) * columnCount + locationY];
                            locationX--;
                        }
                        break;
                    }
                case 3:
                    {
                        if (dicMovable[(locationX+1) * columnCount + locationY])
                        {
                            target = tileList[(locationX+1) * columnCount + locationY];
                            locationX++;
                        }
                        break;
                    }
            }

        }
        
    }

    // 타일 리스트, 이동 가능 체크
    void InitTileList()
    {
        tileList = new List<Transform>();
        dicMovable = new Dictionary<int, bool>();
        for (int i=0; i<tileGrid.childCount; i++)
        {
            tileList.Add(tileGrid.GetChild(i));
            dicMovable.Add(i, false);
        }
        
    }

    // 스테이지 로드
    void InitStage()
    {
        target = tileList[locationX * columnCount + locationY];

        string stageFileName = "STAGE_TEST_2";

        TextAsset textAsset = (TextAsset)Resources.Load(dataPath + stageFileName);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Tile");

        int i = 0;
        string tileName = string.Empty;
        foreach(XmlNode node in nodeList)
        {
            tileName = node.SelectSingleNode("Name").InnerText;

            tileList[i].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("TileSprite/" + tileName, typeof(Sprite));

            tileList[i].eulerAngles = new Vector3(0, 0, float.Parse(node.SelectSingleNode("Rot").InnerText));

            if(tileName.Equals("Default_Sprite"))
            {
                dicMovable[i] = true;
            }
            else
            {
                dicMovable[i] = false;
            }
            i++;
        }
        Debug.Log("Stage Load Success");
    }

    // 방향키 입력
    public void PacLeft()
    {
        inputDirect = 0;
    }

    public void PacRight()
    {
        inputDirect = 1;
    }

    public void PacUp()
    {
        inputDirect = 2;
    }

    public void PacDown()
    {
        inputDirect = 3;
    }
 



}
