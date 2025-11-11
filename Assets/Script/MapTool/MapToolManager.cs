
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using UnityEngine.UI;
using GGame;
using GUtility;

public class MapToolManager : MonoBehaviour
{
    private static MapToolManager _instance = null;
    public static MapToolManager Instance => _instance;

    [SerializeField] private Text stageFileText;
    [SerializeField] private Transform tileGrid;

    [SerializeField] private SpriteManager spriteManager;

    private TileEvent[,] tileArray;
   
    private const int line = 27;
    private const int column = 21;
    private int primLine, primColumn;
    
    private void Awake()
    {
        //싱글톤 초기화
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
    private void Start()
    {
        primLine = (line / 2);
        primColumn = (column / 2);

        tileArray = new TileEvent[line, column];
        
        for (int i = 0;  i < tileGrid.childCount; i++)
        {
            tileArray[i / column, i % column] = tileGrid.GetChild(i).GetComponent<TileEvent>();
            tileArray[i / column, i % column].row = i / column;
            tileArray[i / column, i % column].col = i % column;
        }
    }

    // 맵 초기화
    private void InitMap()
    {
        for(int row = 0; row < line; row++)
        {
            for(int col = 0; col < column; col++)
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
            var xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            var root = xmlDoc.CreateNode(XmlNodeType.Element, "Map", string.Empty);
            xmlDoc.AppendChild(root);

            for (int row = 0; row < line; row++)
            {
                for (int col = 0; col < column; col++)
                {
                    var tileInfo = tileArray[row, col];

                    //오브젝트 종류 분기
                    var node = xmlDoc.CreateNode(XmlNodeType.Element, "Default", string.Empty);
                    switch (tileInfo.objectType)
                    {
                        case EObjectType.WALL:
                            {
                                if(tileInfo.objectNumber != (int)EWall.DEFAULT)
                                {
                                    //지형 정보
                                    node = xmlDoc.CreateNode(XmlNodeType.Element, "Wall", string.Empty);
                                    root.AppendChild(node);

                                    var rot = xmlDoc.CreateElement("Rot");
                                    rot.InnerText = tileArray[row, col].transform.eulerAngles.z.ToString();
                                    node.AppendChild(rot);   
                                }
                                break;
                            }
                        case EObjectType.ITEM:
                            {
                                //아이템 정보
                                node = xmlDoc.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                                root.AppendChild(node);
                                break;
                            }
                        case EObjectType.CHARACTER:
                            {
                                //캐릭터 스테이지 초기 위치 정보
                                node = xmlDoc.CreateNode(XmlNodeType.Element, "Character", string.Empty);
                                root.AppendChild(node);
                                break;
                            }
                    }

                    //공통 데이터
                    var objectNumber = xmlDoc.CreateElement("Number");
                    objectNumber.InnerText = tileInfo.objectNumber.ToString();
                    node.AppendChild(objectNumber);

                    var objectRow = xmlDoc.CreateElement("Row");
                    objectRow.InnerText = (row + 1).ToString();     //메인 게임에선 행과 열이 1,1에서 시작하기 때문에 1씩 더한다.
                    node.AppendChild(objectRow);

                    var objectColumn = xmlDoc.CreateElement("Column");
                    objectColumn.InnerText = (col + 1).ToString();
                    node.AppendChild(objectColumn);
                }
            }
            // 저장
            xmlDoc.Save(filePath);
            //Debug.Log("Data Save Success!");
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
            var fileName = filePath.Substring(filePath.LastIndexOfAny("/".ToCharArray()) + 1);
            fileName = fileName.Substring(0, fileName.Length - 4);
            stageFileText.text = fileName;

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            InitMap();

            //벽 오브젝트 로드
            var nodeList = xmlDoc.SelectNodes("Map/Wall");
            foreach (XmlNode node in nodeList)
            {
                var row = node?.GetNode("Row")?.GetInt() ?? 0;
                var col = node?.GetNode("Column")?.GetInt() ?? 0;
                var number = node?.GetNode("Number")?.GetInt() ?? 0;
                var rot = node?.GetNode("Rot")?.GetFloat() ?? 0f;
                
                tileArray[row - 1, col - 1].transform.eulerAngles = new Vector3(0f, 0f, rot);
                tileArray[row - 1, col - 1].InitTile(EObjectType.WALL, (EWall)number, spriteManager.wallSpriteArray[number]);
            }

            // 아이템 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Item");
            foreach(XmlNode node in nodeList)
            {
                var row = node?.GetNode("Row")?.GetInt() ?? 0;
                var col = node?.GetNode("Column")?.GetInt() ?? 0;
                var number = node?.GetNode("Number")?.GetInt() ?? 0;
                
                tileArray[row - 1, col - 1].InitTile(EObjectType.ITEM, (EWall)number, spriteManager.itemSpriteArray[number]);
            }

            // 캐릭터 오브젝트 로드
            nodeList = xmlDoc.SelectNodes("Map/Character");
            foreach(XmlNode node in nodeList)
            {
                var row = node?.GetNode("Row")?.GetInt() ?? 0;
                var col = node?.GetNode("Column")?.GetInt() ?? 0;
                var number = node?.GetNode("Number")?.GetInt() ?? 0;
                
                tileArray[row - 1, col - 1].InitTile(EObjectType.CHARACTER, (EWall)number, spriteManager.characterSpriteArray[number]);
            }
            
            //Debug.Log("Data Load Success!");
        }
    }

    // 타일 자동 완성 함수
    public void TileAutoComplete(int row, int col, bool isStandard, bool isDefault) // isStandard -> 배치한 타일을 기준으로 인접한 타일들만 자동완성 하기위한 체크 변수
    {                                                                               // isDefault -> 배치한 타일이 빈칸일때 해당 타일은 자동완성할 필요없기에 체크
        int count = 0;
        bool isLeft = false, isRight = false, isUp = false, isDown = false;

        if (SafeArray(tileArray, row - 1, col) && tileArray[row - 1, col].objectType == (int)EObjectType.WALL && tileArray[row - 1, col].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isUp = true;
        }
        if (SafeArray(tileArray, row + 1, col) && tileArray[row + 1, col].objectType == (int)EObjectType.WALL && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isDown = true;
        }
        if (SafeArray(tileArray, row, col - 1) && tileArray[row, col - 1].objectType == (int)EObjectType.WALL && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isLeft = true;
        }
        if (SafeArray(tileArray, row, col + 1) && tileArray[row, col + 1].objectType == (int)EObjectType.WALL && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
        {
            count++;
            isRight = true;
        }

        if (isDefault == false) // 빈칸이 아니라면 자동완성
        {
            switch (count)
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
        }
        

        //기준 타일에 인접한 타일들 자동완성
        if(isStandard)
        {
            if (isLeft)
            {
                TileAutoComplete(row, col - 1, false, false);
            }
            if(isRight)
            {
                TileAutoComplete(row, col + 1, false, false);
            }
            if(isUp)
            {
                TileAutoComplete(row - 1, col, false, false);
            }
            if(isDown)
            {
                TileAutoComplete(row + 1, col, false, false);
            }
        }
        
    }

    //특정 타일 기준으로 1방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteOne(int row, int col)
    {
        float rot = 0f;
        if (SafeArray(tileArray, row - 1, col) && tileArray[row-1, col].objectType == EObjectType.WALL && tileArray[row-1, col].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 180f;
        }
        if (SafeArray(tileArray, row + 1, col) && tileArray[row + 1, col].objectType == EObjectType.WALL && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 0f;
        }
        if (SafeArray(tileArray, row, col - 1) && tileArray[row, col-1].objectType == EObjectType.WALL && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 270;
        }
        if (SafeArray(tileArray, row, col + 1) && tileArray[row, col+1].objectType == EObjectType.WALL && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
        {
            rot = 90f;
        }

        tileArray[row, col].AutoTile(EObjectType.WALL, EWall.EDGE, spriteManager.wallSpriteArray[(int)EWall.EDGE], rot);
        
    }

    //특정 타일 기준으로 2방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteTwo(int row, int col)
    {
        float rot = 0f;
        bool isLine = false;
        if (SafeArray(tileArray, row - 1, col) && SafeArray(tileArray, row + 1, col))
        {
            if ( tileArray[row - 1, col].objectType == EObjectType.WALL && tileArray[row + 1, col].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != EWall.DEFAULT && tileArray[row + 1, col].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 90f;
                    isLine = true;
                }
            }
        }
        if (SafeArray(tileArray, row, col - 1) && SafeArray(tileArray, row, col + 1))
        {
            if( tileArray[row, col -1].objectType == EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if (tileArray[row, col - 1].objectNumber != EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 0f;
                    isLine = true;
                }
                
            }
        }
        if (SafeArray(tileArray, row - 1, col) && SafeArray(tileArray, row, col + 1))
        {
            if(tileArray[row-1, col].objectType == EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 90f;
                }
            }
        }
        if (SafeArray(tileArray, row - 1, col) && SafeArray(tileArray, row, col - 1))
        {
            if(tileArray[row - 1, col].objectType == EObjectType.WALL && tileArray[row, col - 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row - 1, col].objectNumber != EWall.DEFAULT && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 180f;
                }
            }
        }
        if (SafeArray(tileArray, row + 1, col) && SafeArray(tileArray, row, col + 1))
        {
            if(tileArray[row + 1, col].objectType == EObjectType.WALL && tileArray[row, col + 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row + 1, col].objectNumber != EWall.DEFAULT && tileArray[row, col + 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 360f;
                }
            }
        }
        if (SafeArray(tileArray, row + 1, col) && SafeArray(tileArray, row, col - 1))
        {
            if(tileArray[row + 1, col].objectType == EObjectType.WALL && tileArray[row, col - 1].objectType == (int)EObjectType.WALL)
            {
                if(tileArray[row + 1, col].objectNumber != EWall.DEFAULT && tileArray[row, col - 1].objectNumber != (int)EWall.DEFAULT)
                {
                    rot = 270f;
                }
            }
        }

        if(isLine)
        {
            tileArray[row, col].AutoTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE], rot);
        }
        else
        {
            tileArray[row, col].AutoTile((int)EObjectType.WALL, EWall.CURVE, spriteManager.wallSpriteArray[(int)EWall.CURVE], rot);
        }
    }

    //특정 타일 기준으로 3방향에 벽 오브젝트가 있는 경우의 수
    void TileAutoCompleteThree(int row, int col)
    {
        float rot = 0f;
        if (SafeArray(tileArray, row - 1, col))
        {
            if(tileArray[row - 1, col].objectType == EObjectType.WALL && tileArray[row - 1, col].objectNumber == EWall.DEFAULT)
            {
                rot = 0f;
            }
            else if(tileArray[row-1, col].objectType == EObjectType.ITEM)
            {
                rot = 0f;
            }
        }
        if (SafeArray(tileArray, row + 1, col))
        {
            if(tileArray[row + 1, col].objectType == EObjectType.WALL && tileArray[row + 1, col].objectNumber == EWall.DEFAULT)
            {
                rot = 180f;
            }
            else if(tileArray[row + 1, col].objectType == EObjectType.ITEM)
            {
                rot = 180f;
            }
        }
        if (SafeArray(tileArray, row, col - 1))
        {
            if(tileArray[row, col - 1].objectType == EObjectType.WALL && tileArray[row, col - 1].objectNumber == EWall.DEFAULT)
            {
                rot = 90f;
            }
            else if(tileArray[row, col - 1].objectType == EObjectType.ITEM)
            {
                rot = 90f;
            }
        }
        if (SafeArray(tileArray, row, col + 1))
        {
            if(tileArray[row, col + 1].objectType == EObjectType.WALL && tileArray[row, col + 1].objectNumber == EWall.DEFAULT)
            {
                rot = 270f;
            }
            else if(tileArray[row, col + 1].objectType == EObjectType.ITEM)
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

        tileArray[row, col].AutoTile(EObjectType.WALL, EWall.POP, spriteManager.wallSpriteArray[(int)EWall.POP], rot);
    }


    public void RandomMap()
    {
        //프림 알고리즘
        PrimAlgorithm();
    }

    // 프림 알고리즘으로 미로 데이터를 만든 뒤 맵에 적용
    private void PrimAlgorithm()
    {
        var primNodeList = new List<PrimNode>();
        var primArray = new PrimNode[primLine, primColumn]; 
        
        for(int i = 0; i < primLine; i++)
        {
            for(int j = 0; j < primColumn; j++)
            {
                primArray[i, j].row = i;
                primArray[i, j].col = j;
            }
        }

        // 기준 위치(유령 초기 위치)
        int pivotRow = Random.Range(2, primLine-2);
        int pivotCol = Random.Range(2, primColumn-2);
        
        // 노드 2개는 미리 칸을 뚫어놓는다. (유령 리스폰 지역)
        primArray[pivotRow, pivotCol].isCheck = true;
        primArray[pivotRow, pivotCol + 1].isCheck = true;
        primArray[pivotRow, pivotCol].isNear = true;
        primArray[pivotRow, pivotCol + 1].isNear = true;

        IncreaseNode(pivotRow, pivotCol, primNodeList, primArray);
        IncreaseNode(pivotRow, pivotCol + 1, primNodeList, primArray);

        // 프림 알고리즘을 통해 랜덤으로 미로를 생성
        while(primNodeList.Count > 0)
        {
            int randomIndex = Random.Range(0, primNodeList.Count);
            PrimNode node = primNodeList[randomIndex];

            primArray[node.row, node.col].isCheck = true;

            IncreaseNode(node.row, node.col, primNodeList, primArray);

            primNodeList.RemoveAt(randomIndex);
        }

        //Debug.Log("Prim Algorithm End");
        PrimDataCompatible(primArray, pivotRow, pivotCol);
    }

    //인접한 노드들 체크
    private void IncreaseNode(int row, int col, List<PrimNode> primNodeList, PrimNode[,] primArray)
    {
        //근접한 노드 중 칸이 할당된 노드들의 방향 리스트
        var nearNodeList = new List<int>(4);

        if(col + 1 < primColumn && primArray[row, col + 1].isCheck)
        {
            nearNodeList.Add((int)EDirect.EAST);
        }
        if(col - 1 >= 0 && primArray[row, col - 1].isCheck)
        {
            nearNodeList.Add((int)EDirect.WEST);
        }
        if(row + 1 < primLine && primArray[row + 1, col].isCheck)
        {
            nearNodeList.Add((int)EDirect.SOUTH);
        }
        if(row - 1 >= 0 && primArray[row - 1, col].isCheck)
        {
            nearNodeList.Add((int)EDirect.NORTH);
        }

        //근접 노드 방향 중 랜덤하게 하나의 길목 뚫기
        if(nearNodeList.Count > 0)
        {
            int random = Random.Range(0, nearNodeList.Count);
            switch(nearNodeList[random])
            {
                case (int)EDirect.EAST:
                    {
                        primArray[row, col].isRight = true;
                        break;
                    }
                case (int)EDirect.WEST:
                    {
                        primArray[row, col].isLeft = true;
                        break;
                    }
                case (int)EDirect.NORTH:
                    {
                        primArray[row, col].isUp = true;
                        break;
                    }
                case (int)EDirect.SOUTH:
                    {
                        primArray[row, col].isDown = true;
                        break;
                    }
            }
        }

        //뚫은 칸 기준으로 인접한 노드들 갱신
        if (col + 1 < primColumn && primArray[row, col + 1].isNear == false && primArray[row, col + 1].isCheck == false)
        {
            primArray[row, col + 1].isNear = true;
            primNodeList.Add(primArray[row, col + 1]);
        }
        if (col - 1 >= 0 && primArray[row, col - 1].isCheck == false && primArray[row, col - 1].isNear == false)
        {
            primArray[row, col - 1].isNear = true;
            primNodeList.Add(primArray[row, col - 1]);
        }
        if (row + 1 < primLine && primArray[row + 1, col].isCheck == false && primArray[row + 1, col].isNear == false)
        {
            primArray[row + 1, col].isNear = true;
            primNodeList.Add(primArray[row + 1, col]);
        }
        if (row - 1 >= 0 && primArray[row - 1, col].isCheck == false && primArray[row - 1, col].isNear == false)
        {
            primArray[row - 1, col].isNear = true;
            primNodeList.Add(primArray[row - 1, col]);
        }

    }

    //프림 알고리즘 데이터 팩맨 맵에 호환
    private void PrimDataCompatible(PrimNode[,] primArray, int pivotRow, int pivotCol)
    {
        for (int row = 0; row < line; row++)
        {
            for (int col = 0; col < column; col++)
            {
                tileArray[row, col].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
            }
        }

        for(int i = 0; i < primLine; i++)
        {
            for(int j = 0; j < primColumn; j++)
            {
                tileArray[i * 2 + 1, j * 2 + 1].InitTile();
                if(primArray[i, j].isUp)
                {
                    tileArray[i * 2, j * 2 + 1].InitTile();
                }
                if (primArray[i, j].isDown)
                {
                    tileArray[i * 2 + 2, j * 2 + 1].InitTile();
                }
                if (primArray[i, j].isLeft)
                {
                    tileArray[i * 2 + 1, j * 2].InitTile();
                }
                if (primArray[i, j].isRight)
                {
                    tileArray[i * 2 + 1, j * 2 + 2].InitTile();
                }
            }
        }

        GhostRespawnArea(pivotRow * 2, pivotCol * 2);

        // 타일 자동완성
        for (int i = 0; i < line; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (tileArray[i, j].objectType == (int)EObjectType.WALL && tileArray[i, j].objectNumber == EWall.LINE)
                {
                    TileAutoComplete(i, j, false, false);
                }
            }
        }
    }

    // 유령 리스폰 지역 만들기
    private void GhostRespawnArea(int pivotRow, int pivotCol)
    {
        for (int i = pivotRow - 2; i <= pivotRow + 3; i++)
        {
            for (int j = pivotCol - 2; j <= pivotCol + 4; j++)
            {
                tileArray[i, j].InitTile();
            }
        }

        for (int j = pivotCol - 1; j <= pivotCol + 3; j++)
        {
            tileArray[pivotRow - 1, j].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
            tileArray[pivotRow + 2, j].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
        }

        tileArray[pivotRow, pivotCol - 1].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
        tileArray[pivotRow, pivotCol + 3].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
        tileArray[pivotRow + 1, pivotCol - 1].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);
        tileArray[pivotRow + 1, pivotCol + 3].InitTile((int)EObjectType.WALL, EWall.LINE, spriteManager.wallSpriteArray[(int)EWall.LINE]);

        tileArray[pivotRow - 1, pivotCol].InitTile((int)EObjectType.WALL, EWall.LEFTDOOR, spriteManager.wallSpriteArray[(int)EWall.LEFTDOOR]);
        tileArray[pivotRow - 1, pivotCol + 1].InitTile((int)EObjectType.WALL, EWall.CENTERDOOR, spriteManager.wallSpriteArray[(int)EWall.CENTERDOOR]);
        tileArray[pivotRow - 1, pivotCol + 2].InitTile((int)EObjectType.WALL, EWall.RIGHTDOOR, spriteManager.wallSpriteArray[(int)EWall.RIGHTDOOR]);

    }


    // 배열 유효성 방어 코드
    private bool SafeArray<T>(T[,] array, int row, int col)
    {
        if (array != null && row >= 0 && row < line && col >= 0 && col < column)
        {
            return true;
        }
        else
        {
            return false; 
        }
    }
}
