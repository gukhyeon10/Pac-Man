using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public class ItemManager : MonoBehaviour
{
    public GameTile[,] tileArray;
    public Transform[,] itemArray;

    [SerializeField]
    Transform itemPanel;

    //아이템 생성할 프리팹
    [SerializeField]
    GameObject[] itemPrefabArray = new GameObject[Enum.GetNames(typeof(EItem)).Length];

    //인게임 아이템 POOL
    List<GameObject> itemList = new List<GameObject>();

    int line, column;

    // Start is called before the first frame update
    void Start()
    {
        line = StageManager.Instance.GetLine;
        column = StageManager.Instance.GetColumn;

        itemArray = new Transform[line, column];
    }

    //아이템 초기화
    public void InitItem()
    {
        for(int i=0; i<itemList.Count; i++)
        {
            Destroy(itemList[i]);
        }
        itemList.Clear();
    }

    //아이템 로드
    public int LoadItem(XmlNodeList nodeList)
    {
        int normalCount=0;
        int row, col, objectNumber;
        foreach(XmlNode node in nodeList)
        {
            row = int.Parse(node.SelectSingleNode("Row").InnerText);
            col = int.Parse(node.SelectSingleNode("Column").InnerText);
            objectNumber = int.Parse(node.SelectSingleNode("Number").InnerText);

            //아이템 생성
            GameObject item = Instantiate(itemPrefabArray[objectNumber], tileArray[row,col].transform.position, Quaternion.identity, itemPanel);
            item.GetComponent<ItemEvent>().itemNumber = objectNumber;
            itemList.Add(item);

            if(objectNumber == (int)EItem.NORMAL)
            {
                normalCount++;
            }
        }

        return normalCount;
    }
}
