using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    Transform tileGrid;

    const string dataPath = "MapData/";

    public List<Transform> tileList;
    public Dictionary<int, bool> dicMovable;

    // 위치 좌표 (초기값)
    int locationX = 1;  
    int locationY = 1;

    int moveDirect = 1;  // 0 left   1 right   2 up   3 down
    int inputDirect = 1;

    float speed = 2f;
    bool isTurn = false;

    private static MapManager _instance = null;

    public static MapManager Instance
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

        InitTileList();

        InitStage();
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



}
