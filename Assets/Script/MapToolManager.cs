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

    List<Transform> tileList;
    
    // Start is called before the first frame update
    void Start()
    {
        //그리드 자식 타일 List
        tileList = new List<Transform>();
        for(int i=0; i<tileGrid.childCount; i++)
        {
            tileList.Add(tileGrid.GetChild(i));
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

            for (int i = 0; i < tileList.Count; i++)
            {
                XmlNode childNode = xmlDoc.CreateNode(XmlNodeType.Element, "Tile", string.Empty);
                root.AppendChild(childNode);

                XmlElement name = xmlDoc.CreateElement("Name");
                name.InnerText = tileList[i].GetComponent<SpriteRenderer>().sprite.name;
                childNode.AppendChild(name);

                XmlElement rot = xmlDoc.CreateElement("Rot");
                rot.InnerText = tileList[i].eulerAngles.z.ToString();
                childNode.AppendChild(rot);

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

            int i = 0;
            foreach(XmlNode node in nodeList)
            {
                tileList[i].GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("TileSprite/" + node.SelectSingleNode("Name").InnerText, typeof(Sprite));
                
                tileList[i].eulerAngles = new Vector3(0, 0, float.Parse(node.SelectSingleNode("Rot").InnerText));

                tileList[i++].GetComponent<TileEvent>().InitTile();
            }

            Debug.Log("Data Load Success!");
        }
    }
    
}
