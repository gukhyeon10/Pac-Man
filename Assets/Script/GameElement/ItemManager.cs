using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using GUtility;

namespace GGame
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] private Transform itemPanel;
        [SerializeField] private GameObject[] itemPrefabArray;  //아이템 프리팹
        
        private static ItemManager _instance = null;
        public static ItemManager Instance => _instance;
        
        public GameTile[,] tileArray;
        public Transform[,] itemArray;

        private List<GameObject> itemList = new List<GameObject>();
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            itemArray = new Transform[StageManager.Instance.GetLine, StageManager.Instance.GetColumn];
        }

        //아이템 초기화
        public void InitItem()
        {
            itemPanel.SafeSetActive(true);

            for (int i = 0; i < itemList.Count; i++)
            {
                Destroy(itemList[i]);
            }

            itemList.Clear();
        }

        //아이템 로드
        public int LoadItem(XmlNodeList nodeList)
        {
            int normalCount = 0;

            if (nodeList != null)
            {
                foreach (XmlNode node in nodeList)
                {
                    var row = node?.GetNode("Row")?.GetInt() ?? 0;
                    var col = node?.GetNode("Column")?.GetInt() ?? 0;
                    var number = node?.GetNode("Number")?.GetInt() ?? 0;
                    //아이템 생성
                    var item = Instantiate(itemPrefabArray[number], tileArray[row, col].transform.position, Quaternion.identity, itemPanel);
                    itemList.Add(item);

                    //노말 아이템 개수 누적
                    if (number == (int)EItem.NORMAL)
                    {
                        normalCount++;
                    }
                }   
            }

            return normalCount;
        }

        public void ItemPanelDisable()
        {
            itemPanel.SafeSetActive(false);
        }
    }
}
