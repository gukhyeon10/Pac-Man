﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    Transform tileGrid;

    const string dataPath = "MapData/";

    const int column = 23;
    const int line = 29;   
    public Transform[ , ] tileArray = new Transform[line, column];
    public bool[ , ] movableCheckArray = new bool[line, column];
    
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
        for (int i=0; i<tileGrid.childCount - column; i++)   // 마지막 행 타일들은 유령을 가리기위한 타일이기에 게임에 영향X
        {
            tileArray[(i / column), (i % column)] = tileGrid.GetChild(i);
            movableCheckArray[(i / column), (i % column)] = false;
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

        string tileName = string.Empty;
        

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
                row++;
                col = 1;
            }

        }

        Debug.Log("Stage Load Success");
    }



}