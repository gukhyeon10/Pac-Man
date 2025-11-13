
using GUtility;

namespace GGame
{
    /// <summary>
    /// 맵 정보 모델 베이스 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StageModel<T>
    {
        protected int line => StageManager.line;
        protected int column => StageManager.column;
        
        protected readonly T[][] array;
        
        public T this[int row, int col] => array.IsValidIndex(row, col) ? array[row][col] : default;

        protected StageModel()
        {
            array = new T[line][];
            
            for (int i = 0; i < line; i++)
            {
                array[i] = new T[column];
            }
        }

        public abstract void Init(int tileCount);
        
        public abstract void SetUp(int row, int col, int tileNumber);
    }

    /// <summary>
    /// 타일 이미지 관련 모델
    /// </summary>
    public class TileModel : StageModel<GameTile>
    {
        public void Start(UnityEngine.Transform tileGrid)
        {
            for (int i = 0; i < tileGrid.childCount - column; i++)
            {
                var row = i / column;
                var col =  i % column;
                
                array[row][col] = tileGrid.GetChild(i).GetComponent<GameTile>();
            }
        }
        
        public override void Init(int tileCount)
        {
            var defaultSprite = array[0][0].spriteRenderer.sprite;
            
            for (int row = 0; row < line; row++)
            {
                for (int col = 0; col < column; col++)
                {
                    if (array.IsValidIndex(row, col))
                    {
                        array[row][col].spriteRenderer.sprite = defaultSprite;
                    }
                }
            }
        }

        public override void SetUp(int row, int col, int tileNumber)
        {
            throw new System.NotImplementedException();
        }
    }
    
    /// <summary>
    /// 이동 데이터 모델
    /// </summary>
    public class MoveModel : StageModel<bool>
    {
        public override void Init(int tileCount)
        {
            for (int i = 0; i < tileCount; i++)
            {
                var row = i / column;
                var col =  i % column;

                // 테두리 안 영역인지 체크
                var isInsideArea = row > 0 && row < line - 1 && col > 0 && col < column - 1;

                if (array.IsValidIndex(row, col))
                {
                    array[row][col] = isInsideArea;
                }
            }
        }

        public override void SetUp(int row, int col, int tileNumber)
        {
            if (array.IsValidIndex(row, col))
            {
                array[row][col] = false;
            }
            
            //화면상 테두리부분이 뚫려있다면 테두리 한칸 밖 타일 이동가능
            if (tileNumber == (int)EWall.DEFAULT) // 이동가능 타일
            {
                if (col >= 1 && col < column - 1)
                {
                    if (row == 1)
                    {
                        array[0][col] = true;
                    }

                    if (row == line - 2)
                    {
                        array[line - 1][col] = true;
                    }
                }

                if (row >= 1 && row < line - 1)
                {
                    if (col == 1)
                    {
                        array[row][0] = true;    
                    }

                    if (col == column - 2)
                    {
                        array[row][column - 1] = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 유령 리스폰 이동 데이터 모델
    /// </summary>
    public class GhostRespawnModel : MoveModel
    {
        private (int row, int col) respawnCoord; //유령 디폴트 초기 위치

        public (int row, int col) RespawnCoord => respawnCoord;

        public override void Init(int tileCount)
        {
            for (int i = 0; i < tileCount; i++)
            {
                var row = i / column;
                var col =  i % column;

                if (array.IsValidIndex(row, col))
                {
                    array[row][col] = true;
                }
            }
            
            respawnCoord = (line / 2, column / 2);
        }

        public override void SetUp(int row, int col, int tileNumber)
        {
            if (array.IsValidIndex(row, col))
            {
                if (tileNumber == (int)EWall.CENTERDOOR)
                {
                    array[row][col] = true;
                    
                    respawnCoord = (row, col);
                }
                else
                {
                    array[row][col] = false;
                }
            }
        }
    }
}
