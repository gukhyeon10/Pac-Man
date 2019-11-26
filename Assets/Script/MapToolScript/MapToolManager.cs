﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Xml;
using System;
using UnityEngine.UI;

public class MapToolManager : MonoBehaviour
{
    private static MapToolManager _instance = null;

    public static MapToolManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    Text stageFileText;
    [SerializeField]
    Transform tileGrid;

    [SerializeField]
    SpriteManager spriteManager;

    TileEvent[,] tileArray;
   
    const int line = 27;
    const int column = 21;

    void Awake()
    {
        //싱글톤 초기화
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


    // Start is called before the first frame update
    void Start()
    {
        tileArray = new TileEvent[line, column];
        for(int i=0; i<tileGrid.childCount; i++)
        {
            tileArray[i / column, i % column] = tileGrid.GetChild(i).GetComponent<TileEvent>();
            tileArray[i / column, i % column].row = i / column;
            tileArray[i / column, i % column].col = i % column;
        }
    }

    // 맵 초기화
    void InitMap()
    {
        for(int row =0; row< line; row++)
        {
            for(int col = 0; col<column; col++)
            {
                tileArray[row, col].InitTile();
            }
        }
    }

    
    // 맵 데이터 저장
    public void MapDataSave()
    {
        string filePath = string.Empty;
#if UNITY_EDITOR
        filePath = EditorUtility.SaveFilePanel("Save Map File Dialog", Application.dataPath, "STAGE", "xml");
#endif

        if (filePath.Length > 0)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "Map", string.Empty);
            xmlDoc.AppendChild(root);

            for (int row = 0; row < line; row++)
            {
                for (int col = 0; col < column; col++)
                {
                    TileEvent tileInfo = tileArray[row, col];

                    //오브젝트 종류 분기
                    XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, "Default", string.Empty);
                    switch (tileInfo.objectType)
                    {
                        case (int)EObjectType.WALL:
                            {
                                if(tileInfo.objectNumber != (int)EWall.DEFAULT)
                                {
                                    //지형 정보
                                    node = xmlDoc.CreateNode(XmlNodeType.Element, "Wall", string.Empty);
                                    root.AppendChild(node);

                                    XmlElement rot = xmlDoc.CreateElement("Rot");
                                    rot.InnerText = tileArray[row, col].transform.eulerAngles.z.ToString();
                                    node.AppendChild(rot);   
                                }
                                break;
                            }
                        case (int)EObjectType.ITEM:
                            {
                                //아이템 정보
                                node = xmlDoc.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                                root.AppendChild(node);
                                break;
                            }
                        case (int)EObjectType.CHARACTER:
                            {
                                //캐릭터 스테이지 초기 위치 정보
                                node = xmlDoc.CreateNode(XmlNodeType.Element, "Character", string.Empty);
                                root.AppendChild(node);
                                break;
                            }
                    }

                    //공통 데이터
                    XmlElement objectNumber = xmlDoc.CreateElement("Number");
                    objectNumber.InnerText = tileInfo.objectNumber.ToString();
                    node.AppendChild(objectNumber);

                    XmlElement objectRow = xmlDoc.CreateElement("Row");
                    objectRow.InnerText = (row + 1).ToString();     //메인 게임에선 행과 열이 1,1에서 시작하기 때문에 1씩 더한다.
                    node.AppendChild(objectRow);

                    XmlElement objectColumn = xmlDoc.CreateElement("Column");
                    objectColumn.InnerText = (col + 1).ToString();
                    node.AppendChild(objectColumn);
                }
            }
            // 저장
            xmlDoc.Save(filePath);
            Debug.Log("Data Save Success!");
        }

    }

    // 맵 데이터 로드
    public void MapDataLoad()
    {
        string filePath = string.Empty;
#if UNITY_EDITOR
        filePath = EditorUtility.OpenFilePanel("Load Map File Dialog", Application.dataPath, "xml");
#endif

        if (filePath.Length > 0)
        {
            //파일 이름 표시
            string fileName = filePath.Substring(filePath.LastIndexOfAny("/".ToCharArray()) + 1);
            fileName = fileName.Substring(0, fileName.Length - 4);
            stageFileText.text = fileName;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            InitMap();

            int row, col, objectNumber;
            float rot;

            //벽 오브젝트 로드
            XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Wall");
            foreach (XmlNode node in nodeList)
            {
                objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
                row = int.Parse(node.SelectSingleNode("Row").InnerText);
                col = int.Parse(node.SelectSingleNode("Column").InnerText);
                rot = float.Parse(node.SelectSingleNode("Rot").InnerText);
                
                tileArray[row - 1, col - 1].transform.eulerAngles = new Vector3(0f, 0f, rot);
                tileArray[row - 1, col - 1].InitTile((int)EObjectType.WALL, objectNumber, spriteManager.wallSpriteArray[objectNumber]);

            }

            // 아이템 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Item");
            foreach(XmlNode node in nodeList)
            {
                objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
                row = int.Parse(node.SelectSingleNode("Row").InnerText);
                col = int.Parse(node.SelectSingleNode("Column").InnerText);
                
                tileArray[row - 1, col - 1].InitTile((int)EObjectType.ITEM, objectNumber, spriteManager.itemSpriteArray[objectNumber]);
            }

            // 캐릭터 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Character");
            foreach(XmlNode node in nodeList)
            {
                objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
                row = int.Parse(node.SelectSingleNode("Row").InnerText);
                col = int.Parse(node.SelectSingleNode("Column").InnerText);
                
                tileArray[row - 1, col - 1].InitTile((int)EObjectType.CHARACTER, objectNumber, spriteManager.characterSpriteArray[objectNumber]);
            }
            
            Debug.Log("Data Load Success!");
        }
    }

    // 타일 자동 완성 함수
    public void TileAutoComplete(int row, int col, bool isStandard) // isStandard -> 배치한 타일을 기준으로 인접한 타일들만 자동완성 하기위한 체크 변수
    {
        int count = 0;
        bool isLeft = false, isRight = false, isUp = false, isDown = false;
        if(SafeArray<TileEvent>(tileArray, row - 1, col) && tileArray[row - 1, col].objectType == (int)EObjectType.WALL && tileArray[row - 1,col].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isUp = true;
        }
        if(SafeArray<TileEvent>(tileArray, row + 1, col) && tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isDown = true;
        }
        if(SafeArray<TileEvent>(tileArray, row, col - 1) && tileArray[row, col - 1].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isLeft = true;
        }
        if(SafeArray<TileEvent>(tileArray, row, col + 1) && tileArray[row, col + 1].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isRight = true;
        }

        switch(count)
        {
            case 1:
                {
                    TileAutoCompleteOne(row, col);
                    break;
                }
            case 2:
                {
                    TileAutoCompleteTwo(row, col);
                    break;
                }
            case 3:
                {
                    TileAutoCompleteThree(row, col);
                    break;
                }
        }

        //기준 타일에 인접한 타일들 자동완성
        if(isStandard)
        {
            if (isLeft)
            {
                TileAutoComplete(row, col - 1, false);
            }
            if(isRight)
            {
                TileAutoComplete(row, col + 1, false);
            }
            if(isUp)
            {
                TileAutoComplete(row - 1, col, false);
            }
            if(isDown)
            {
                TileAutoComplete(row + 1, col, false);
            }
        }
        
    }

    //특정 타일 기준으로 1방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteOne(int row, int col)
    {
        float rot = 0f;
        if (SafeArray<TileEvent>(tileArray, row - 1, col) && tileArray[row-1, col].objectType == (int)EObjectType.WALL && tileArray[row-1, col].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 180f;
        }
        if (SafeArray<TileEvent>(tileArray, row + 1, col) && tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 0f;
        }
        if (SafeArray<TileEvent>(tileArray, row, col - 1) && tileArray[row, col-1].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 270;
        }
        if (SafeArray<TileEvent>(tileArray, row, col + 1) && tileArray[row, col+1].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 90f;
        }

        tileArray[row, col].AutoTile((int)EObjectType.WALL, (int)EWall.EDGE, spriteManager.wallSpriteArray[(int)EWall.EDGE], rot);
        
    }

    //특정 타일 기준으로 2방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteTwo(int row, int col)
    {
        float rot = 0f;
        bool isLine = false;
        if (SafeArray<TileEvent>(tileArray, row - 1, col) && SafeArray<TileEvent>(tileArray, row + 1, col))
        {
            if ( tileArray[row - 1, col].objectType == (int)EObjectType.WALL && tileArray[row + 1, col].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != (int)EWall.DEFAULT && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 90f;
                    isLine = true;
                }
            }
        }
        if (SafeArray<TileEvent>(tileArray, row, col - 1) && SafeArray<TileEvent>(tileArray, row, col + 1))
        {
            if( tileArray[row, col -1].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if (tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 0f;
                    isLine = true;
                }
                
            }
        }
        if (SafeArray<TileEvent>(tileArray, row - 1, col) && SafeArray<TileEvent>(tileArray, row, col + 1))
        {
            if(tileArray[row-1, col].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != (int)EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 90f;
                }
            }
        }
        if (SafeArray<TileEvent>(tileArray, row - 1, col) && SafeArray<TileEvent>(tileArray, row, col - 1))
        {
            if(tileArray[row - 1, col].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != (int)EWall.DEFAULT && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 180f;
                }
            }
        }
        if (SafeArray<TileEvent>(tileArray, row + 1, col) && SafeArray<TileEvent>(tileArray, row, col + 1))
        {
            if(tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 360f;
                }
            }
        }
        if (SafeArray<TileEvent>(tileArray, row + 1, col) && SafeArray<TileEvent>(tileArray, row, col - 1))
        {
            if(tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 270f;
                }
            }
        }

        if(isLine)
        {
            tileArray[row, col].AutoTile((int)EObjectType.WALL, (int)EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE], rot);
        }
        else
        {
            tileArray[row, col].AutoTile((int)EObjectType.WALL, (int)EWall.CURVE, spriteManager.wallSpriteArray[(int)EWall.CURVE], rot);
        }
    }

    //특정 타일 기준으로 3방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteThree(int row, int col)
    {
        float rot = 0f;
        if (SafeArray<TileEvent>(tileArray, row - 1, col))
        {
            if(tileArray[row - 1, col].objectType == (int)EObjectType.WALL && tileArray[row - 1, col].objectNumber == (int)EWall.DEFAULT)
            {
                rot = 0f;
            }
            else if(tileArray[row-1, col].objectType == (int)EObjectType.ITEM)
            {
                rot = 0f;
            }
        }
        if (SafeArray<TileEvent>(tileArray, row + 1, col))
        {
            if(tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row + 1, col].objectNumber == (int)EWall.DEFAULT)
            {
                rot = 180f;
            }
            else if(tileArray[row + 1, col].objectType == (int)EObjectType.ITEM)
            {
                rot = 180f;
            }
        }
        if (SafeArray<TileEvent>(tileArray, row, col - 1))
        {
            if(tileArray[row, col - 1].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectNumber == (int)EWall.DEFAULT)
            {
                rot = 90f;
            }
            else if(tileArray[row, col - 1].objectType == (int)EObjectType.ITEM)
            {
                rot = 90f;
            }
        }
        if (SafeArray<TileEvent>(tileArray, row, col + 1))
        {
            if(tileArray[row, col + 1].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectNumber == (int)EWall.DEFAULT)
            {
                rot = 270f;
            }
            else if(tileArray[row, col + 1].objectType == (int)EObjectType.ITEM)
            {
                rot = 270f;
            }
        }

        if (col + 1 == column)  // 우측 테두리
        {
            rot = 270f;
        }
        else if (col == 0)    // 좌측 테두리
        {
            rot = 90f;
        }
        else if (row + 1 == line)  // 하단 테두리
        {
            rot = 180f;
        }
        else if( row == 0)        // 상단 테두리
        {
            rot = 0f;
        }

        tileArray[row, col].AutoTile((int)EObjectType.WALL, (int)EWall.POP, spriteManager.wallSpriteArray[(int)EWall.POP], rot);
    }

    // 배열 유효성 방어 코드
    bool SafeArray<T>(T[,] array, int row, int col)
    {
        if (array != null && row >= 0 && row < line && col >=0 && col < column)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
