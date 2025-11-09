
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class CharacterBase : MonoBehaviour
    {
        protected GameTile[,] tileArray;
        protected bool[,] movableCheckArray; // 캐릭터 이동 가능 좌표 플래그
        protected bool[,] ghostRespawnMovableCheckArray; // 유령 리스폰 경로 이동 가능 좌표 플래그

        //이동 목표 Transform
        protected Transform target;

        protected Transform character;

        //팩맨 캐릭터
        public static PacMan pac;

        //타일 29행 23열
        protected const int column = 23;
        protected const int line = 29;

        protected readonly BfsNode[,] bfsArray = new BfsNode[line, column]; // Bfs 탐색 array
        protected readonly Queue<BfsNode> bfsQueue = new Queue<BfsNode>(); // Bfs 탐색 queue

        //목표 위치 좌표
        protected (int row, int col) coord;
        public (int row, int col) Coord => coord;
        
        protected bool isRespawn = false;
        protected bool isReturn = false;

        protected int moveDirect = (int)EDirect.EAST;
        protected float speed = 2f;
        public bool isContinue = true;

        public Animator animator;
        public BoxCollider2D boxCollider;

        public void InitCharacter()
        {
            character = this.transform;
            tileArray = StageManager.Instance.tileArray;
            movableCheckArray = StageManager.Instance.movableCheckArray;
            ghostRespawnMovableCheckArray = StageManager.Instance.ghostRespawnMovableCheckArray;

            coord.row = 2;
            coord.col = 2;

            target = tileArray[coord.row, coord.col].transform;
            character.position = target.position;

            isRespawn = false;
            isReturn = false;
        }

        // 캐릭터 초기화 오버로딩
        public void InitCharacter(int x, int y)
        {
            character = this.transform;
            tileArray = StageManager.Instance.tileArray;
            movableCheckArray = StageManager.Instance.movableCheckArray;
            ghostRespawnMovableCheckArray = StageManager.Instance.ghostRespawnMovableCheckArray;

            coord.row = x;
            coord.col = y;

            target = tileArray[coord.row, coord.col].transform;
            character.position = target.position;

            isRespawn = false;
            isReturn = false;
            boxCollider.enabled = true;
        }

        /// <summary>
        /// 바라보는 방향으로 계속 가는지 검사
        /// </summary>
        protected bool IsKeepMove(EDirect direct)
        {
            return direct switch
            {
                EDirect.EAST => movableCheckArray[coord.row, coord.col + 1],
                EDirect.WEST => movableCheckArray[coord.row, coord.col - 1],
                EDirect.SOUTH => movableCheckArray[coord.row + 1, coord.col],
                EDirect.NORTH => movableCheckArray[coord.row - 1, coord.col],
                
                _ => true,
            };
        }
        
        /// <summary>
        /// 이동 좌표 갱신 
        /// </summary>
        protected void UpdateCoord()
        {
            coord = moveDirect switch
            {
                (int)EDirect.EAST  => (coord.row, coord.col + 1),
                (int)EDirect.WEST  => (coord.row, coord.col - 1),
                (int)EDirect.SOUTH => (coord.row + 1, coord.col),
                (int)EDirect.NORTH => (coord.row - 1, coord.col),
                                
                _ => (coord.row, coord.col)
            };
        }

        /// <summary>
        /// 이동 방향 갱신
        /// </summary>
        private void UpdateDirect()
        {
            var movableList = new List<int>(4); // 갈 수 있는 방향 리스트
            if (movableCheckArray[coord.row, coord.col + 1])
            {
                movableList.Add((int)EDirect.EAST);
            }

            if (movableCheckArray[coord.row, coord.col - 1])
            {
                movableList.Add((int)EDirect.WEST);
            }

            if (movableCheckArray[coord.row + 1, coord.col])
            {
                movableList.Add((int)EDirect.SOUTH);
            }

            if (movableCheckArray[coord.row - 1, coord.col])
            {
                movableList.Add((int)EDirect.NORTH);
            }

            if (movableList.Count > 0)
            {
                int index = Random.Range(0, movableList.Count);
                switch (movableList[index])
                {
                    case (int)EDirect.WEST:
                    {
                        coord.col--;
                        moveDirect = (int)EDirect.WEST;
                        break;
                    }
                    case (int)EDirect.EAST:
                    {
                        coord.col++;
                        moveDirect = (int)EDirect.EAST;
                        break;
                    }
                    case (int)EDirect.NORTH:
                    {
                        coord.row--;
                        moveDirect = (int)EDirect.NORTH;
                        break;
                    }
                    case (int)EDirect.SOUTH:
                    {
                        coord.row++;
                        moveDirect = (int)EDirect.SOUTH;
                        break;
                    }
                }
            }
        }

        // 각 캐릭터 별 재정의 함수
        protected virtual void CharacterMove()
        {
            // 기본 움직임 (랜덤 방향)
            if (!SafeTarget(target)) // 타겟 null 처리
            {
                return;
            }

            if (Vector3.Distance(character.position, target.position) < 0.01f)
            {
                if (!WrapCoordinate())
                {
                    if (IsKeepMove((EDirect)moveDirect))
                    {
                        UpdateCoord();
                    }
                    else
                    {
                        UpdateDirect();
                    }

                    target = StageManager.SafeArray(tileArray, coord.row, coord.col) ? tileArray[coord.row, coord.col].transform : null;
                }
            }

            if (target != null)
            {
                character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);   
            }
        }

        /// <summary>
        /// 맵 끝으로 이동시 반대편으로 이동
        /// </summary>
        protected bool WrapCoordinate()
        {
            if (coord.row == 0)
            {
                character.position = tileArray[line - 1, coord.col].transform.position;
                coord.row = line - 2;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.row == line - 1)
            {
                character.position = tileArray[0, coord.col].transform.position;
                coord.row = 1;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.col == 0)
            {
                character.position = tileArray[coord.row, column - 1].transform.position;
                coord.col = column - 2;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.col == column - 1)
            {
                character.position = tileArray[coord.row, 0].transform.position;
                coord.col = 1;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else
            {
                return false;
            }
        }

        // 타겟 방어 코드
        protected bool SafeTarget<T>(T target)
        {
            if (target != null)
            {
                return true;
            }
            else
            {
                DebugHelper.Log(DebugHelper.DEBUG.NULL);

                return false;
            }
        }
    }
}