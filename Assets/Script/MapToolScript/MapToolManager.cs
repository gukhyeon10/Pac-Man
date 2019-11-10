using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Xml;
using System;

public class MapToolManager : MonoBehaviour
{
    [SerializeField]
    Transform tileGrid;


    [SerializeField]
    Sprite[] wallSpriteArray = new Sprite[Enum.GetNames(typeof(EWall)).Length];
    [SerializeField]
    Sprite[] itemSpriteArray = new Sprite[Enum.GetNames(typeof(EItem)).Length];
    [SerializeField]
    Sprite[] characterSpriteArray = new Sprite[Enum.GetNames(typeof(ECharacter)).Length];

    Transform[,] tileArray;


    const int line = 27;
    const int column = 21;
    
    // Start is called before the first frame update
    void Start()
    {
        tileArray = new Transform[line, column];
        for(int i=0; i<tileGrid.childCount; i++)
        {
            tileArray[i / column, i % column] = tileGrid.GetChild(i);
        }
    }
    
    // 맵 초기화
    void InitMap()
    {
        for(int row =0; row< line; row++)
        {
            for(int col = 0; col<column; col++)
            {
                tileArray[row, col].GetComponent<TileEvent>().InitTile();
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
                    TileEvent tileInfo = tileArray[row, col].GetComponent<TileEvent>();
                    switch (tileInfo.objectType)
                    {
                        case (int)EObjectType.WALL:
                            {
                                if(tileInfo.objectNumber != (int)EWall.DEFAULT)
                                {
                                    //지형 정보
                                    XmlNode wallNode = xmlDoc.CreateNode(XmlNodeType.Element, "Wall", string.Empty);
                                    root.AppendChild(wallNode);

                                    XmlElement wallNumber = xmlDoc.CreateElement("Number");
                                    wallNumber.InnerText = tileInfo.objectNumber.ToString();
                                    wallNode.AppendChild(wallNumber);

                                    XmlElement wallLine = xmlDoc.CreateElement("Row");
                                    wallLine.InnerText = (row + 1).ToString();     //메인 게임에선 행과 열이 1,1에서 시작하기 때문에 1씩 더한다.
                                    wallNode.AppendChild(wallLine);

                                    XmlElement itemColumn = xmlDoc.CreateElement("Column");
                                    itemColumn.InnerText = (col + 1).ToString();
                                    wallNode.AppendChild(itemColumn);

                                    XmlElement rot = xmlDoc.CreateElement("Rot");
                                    rot.InnerText = tileArray[row, col].eulerAngles.z.ToString();
                                    wallNode.AppendChild(rot);
                                    
                                }
                                break;
                            }
                        case (int)EObjectType.ITEM:
                            {
                                //아이템 정보
                                XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                                root.AppendChild(itemNode);

                                XmlElement itemNumber = xmlDoc.CreateElement("Number");
                                itemNumber.InnerText = tileInfo.objectNumber.ToString();
                                itemNode.AppendChild(itemNumber);

                                XmlElement itemLine = xmlDoc.CreateElement("Row");
                                itemLine.InnerText = (row + 1).ToString();     //메인 게임에선 행과 열이 1,1에서 시작하기 때문에 1씩 더한다.
                                itemNode.AppendChild(itemLine);

                                XmlElement itemColumn = xmlDoc.CreateElement("Column");
                                itemColumn.InnerText = (col + 1).ToString();
                                itemNode.AppendChild(itemColumn);

                                break;
                            }
                        case (int)EObjectType.CHARACTER:
                            {
                                //캐릭터 스테이지 초기 위치 정보
                                XmlNode characterNode = xmlDoc.CreateNode(XmlNodeType.Element, "Character", string.Empty);
                                root.AppendChild(characterNode);

                                XmlElement characterNumber = xmlDoc.CreateElement("Number");
                                characterNumber.InnerText = tileInfo.objectNumber.ToString();
                                characterNode.AppendChild(characterNumber);

                                XmlElement characterLine = xmlDoc.CreateElement("Row");
                                characterLine.InnerText = (row + 1).ToString();
                                characterNode.AppendChild(characterLine);

                                XmlElement characterColumn = xmlDoc.CreateElement("Column");
                                characterColumn.InnerText = (col + 1).ToString();
                                characterNode.AppendChild(characterColumn);

                                break;
                            }
                    }
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

                //tileArray[row - 1, col - 1].GetComponent<SpriteRenderer>().sprite = wallSpriteArray[objectNumber];
                tileArray[row - 1, col - 1].eulerAngles = new Vector3(0f, 0f, rot);
                tileArray[row - 1, col - 1].GetComponent<TileEvent>().InitTile((int)EObjectType.WALL, objectNumber, wallSpriteArray[objectNumber]);

            }

            // 아이템 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Item");
            foreach(XmlNode node in nodeList)
            {
                objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
                row = int.Parse(node.SelectSingleNode("Row").InnerText);
                col = int.Parse(node.SelectSingleNode("Column").InnerText);

               // tileArray[row - 1, col - 1].GetComponent<SpriteRenderer>().sprite = itemSpriteArray[objectNumber];
                tileArray[row - 1, col - 1].GetComponent<TileEvent>().InitTile((int)EObjectType.ITEM, objectNumber, itemSpriteArray[objectNumber]);
            }

            // 캐릭터 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Character");
            foreach(XmlNode node in nodeList)
            {
                objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);
                row = int.Parse(node.SelectSingleNode("Row").InnerText);
                col = int.Parse(node.SelectSingleNode("Column").InnerText);

                //tileArray[row - 1, col - 1].GetComponent<SpriteRenderer>().sprite = characterSpriteArray[objectNumber];
                tileArray[row - 1, col - 1].GetComponent<TileEvent>().InitTile((int)EObjectType.CHARACTER, objectNumber, characterSpriteArray[objectNumber]);
            }
            
            Debug.Log("Data Load Success!");
        }
    }
    
}
