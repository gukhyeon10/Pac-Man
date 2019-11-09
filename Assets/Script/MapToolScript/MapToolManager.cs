using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Xml;

public class MapToolManager : MonoBehaviour
{
    [SerializeField]
    Transform tileGrid;
    
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
                                //지형 정보
                                XmlNode tileNode = xmlDoc.CreateNode(XmlNodeType.Element, "Tile", string.Empty);
                                root.AppendChild(tileNode);
                                
                                XmlElement name = xmlDoc.CreateElement("Name");
                                name.InnerText = tileArray[row,col].GetComponent<SpriteRenderer>().sprite.name;
                                tileNode.AppendChild(name);

                                XmlElement rot = xmlDoc.CreateElement("Rot");
                                rot.InnerText = tileArray[row,col].eulerAngles.z.ToString();
                                tileNode.AppendChild(rot);
                                break;
                            }
                        case (int)EObjectType.ITEM:
                            {
                                //기본 지형
                                XmlNode tileNode = xmlDoc.CreateNode(XmlNodeType.Element, "Tile", string.Empty);
                                root.AppendChild(tileNode);

                                XmlElement tileName = xmlDoc.CreateElement("Name");
                                tileName.InnerText = "Default_Sprite";
                                tileNode.AppendChild(tileName);

                                XmlElement rot = xmlDoc.CreateElement("Rot");
                                rot.InnerText = "0";
                                tileNode.AppendChild(rot);

                                //아이템 정보
                                XmlNode itemNode = xmlDoc.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                                root.AppendChild(itemNode);

                                XmlElement itemName = xmlDoc.CreateElement("Name");
                                itemName.InnerText = tileArray[row,col].GetComponent<SpriteRenderer>().sprite.name;
                                itemNode.AppendChild(itemName);

                                XmlElement itemLine = xmlDoc.CreateElement("Line");
                                itemLine.InnerText = (row + 1).ToString();
                                itemNode.AppendChild(itemLine);

                                XmlElement itemColumn = xmlDoc.CreateElement("Column");
                                itemColumn.InnerText = (col + 1).ToString();
                                itemNode.AppendChild(itemColumn);

                                break;
                            }
                        case (int)EObjectType.CHARACTER:
                            {
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

            XmlNodeList nodeList = xmlDoc.SelectNodes("Map/Tile");

            //타일 로드
            int i = 0;
            foreach(XmlNode node in nodeList)
            {   
                tileArray[i/column, i%column].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("TileSprite/" + node.SelectSingleNode("Name").InnerText, typeof(Sprite));

                tileArray[i/column, i%column].eulerAngles = new Vector3(0, 0, float.Parse(node.SelectSingleNode("Rot").InnerText));

                tileArray[i/column, i%column].GetComponent<TileEvent>().InitTile((int)EObjectType.WALL);
                i++;
            }
            
            //아이템 로드
            int row, col;
            nodeList = xmlDoc.SelectNodes("Map/Item");
            foreach(XmlNode node in nodeList)
            {
                row = int.Parse(node.SelectSingleNode("Line").InnerText) - 1;
                col = int.Parse(node.SelectSingleNode("Column").InnerText) - 1;
                tileArray[row, col].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("ItemSprite/" + node.SelectSingleNode("Name").InnerText, typeof(Sprite));
                tileArray[row, col].GetComponent<TileEvent>().InitTile((int)EObjectType.ITEM);
            }

            Debug.Log("Data Load Success!");
        }
    }
    
}
