using System.Collections;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class Ghost : CharacterBase
    {
        private bool isLookPac = false;

        protected virtual float RespawnCoolTime => 3f;
        protected virtual float TrackingTime => 5f;
        
        private void OnEnable()
        {
            isLookPac = false;
            
            StopAllCoroutines();
            
            StartCoroutine(GhostRespawnCoolTime());
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isContinue)
            {
                return;
            }

            animator.SetInteger("DIRECT", moveDirect);

            if (isRespawn)
            {
                GhostRespawn();
            }
            else if (isReturn)
            {
                GhostReturn();
            }
            else
            {
                //팩맨 슈퍼모드일 경우 유령 모습 변화
                if (pac.GetIsSuperMode)
                {
                    animator.SetBool("SCARE", true);
                }
                else
                {
                    animator.SetBool("SCARE", false);
                }

                if (isLookPac == false && GhostLookPac())
                {
                    isLookPac = true;
                    
                    StartCoroutine(TrackingPacMan());
                }

                if (isLookPac)
                {
                    PathTracking();
                }
                else
                {
                    CharacterMove();
                }
            }
        }
        
        /// <summary>
        /// 타겟 좌표 경로 탐색 시작
        /// </summary>
        private void PathFinding((int row, int col) coord, (int row, int col) targetCoord)
        {
            bfsArray[coord.row, coord.col].Modify(coord.row, coord.col, coord.row, coord.col, true);

            bfsQueue.Clear();
            bfsQueue.Enqueue(bfsArray[coord.row, coord.col]);

            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();
                if (node.Row == targetCoord.row && node.Col == targetCoord.col)
                {
                    BackTracking(node.Row, node.Col);
                    break;
                }

                PathFindingProcess(node.Row - 1, node.Col, node.Row, node.Col);

                PathFindingProcess(node.Row + 1, node.Col, node.Row, node.Col);

                PathFindingProcess(node.Row, node.Col - 1, node.Row, node.Col);

                PathFindingProcess(node.Row, node.Col + 1, node.Row, node.Col);
            }
        }

        /// <summary>
        /// 경로 탐색 처리
        /// </summary>
        private void PathFindingProcess(int destRow, int destCol, int srcRow, int srcCol)
        {
            if (IsMovable(destRow, destCol) && bfsArray[destRow, destCol].IsVisited == false)
            {
                bfsArray[destRow, destCol].Modify(destRow, destCol, srcRow, srcCol, true);

                bfsQueue.Enqueue(bfsArray[destRow, destCol]);
            }
        }

        /// <summary>
        /// 이동 가능한 좌표인지 검사
        /// </summary>
        private bool IsMovable(int row, int col)
        {
            if (row >= 0 && col >= 0 && row < line && col < column)
            {
                if (movableCheckArray[row, col])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 타겟 좌표까지의 경로 중 첫번째 좌표를 역추적
        /// </summary>
        private void BackTracking(int reverseRow, int reverseCol)
        {
            var loopSafeCount = 0;
            var maxSafeCount = column * line * 2;

            while (!(reverseRow == coord.row && reverseCol == coord.col))
            {
                int preRow = bfsArray[reverseRow, reverseCol].PreRow;
                int preCol = bfsArray[reverseRow, reverseCol].PreCol;

                if (preRow == coord.row && preCol == coord.col)
                {
                    GhostTurn(reverseRow, reverseCol, coord.row, coord.col);

                    coord.row = reverseRow;
                    coord.col = reverseCol;

                    return;
                }
                else
                {
                    reverseRow = preRow;
                    reverseCol = preCol;
                }

                if (++loopSafeCount > maxSafeCount)
                {
                    DebugHelper.Log($"무한 루프 방지 {loopSafeCount}");

                    break;
                }
            }
        }
        
        /// <summary>
        /// bfs 탐색 시작 전 초기화
        /// </summary>
        /// <param name="movableCheckArray"></param>
        private void InitBfsArray(bool[,] movableCheckArray = null)
        {
            this.movableCheckArray = movableCheckArray ?? this.movableCheckArray;

            for (int i = 0; i < line; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    bfsArray[i, j].Modify(i, j, i, j, false);
                }
            }
        }
        
        /// <summary>
        /// 방향 전환
        /// </summary>
        private void GhostTurn(int targetRow, int targetCol, int pivotRow, int pivotCol)
        {
            if (targetRow == pivotRow - 1)
            {
                moveDirect = (int)EDirect.NORTH;
            }
            else if (targetRow == pivotRow + 1)
            {
                moveDirect = (int)EDirect.SOUTH;
            }
            else if (targetCol == pivotCol - 1)
            {
                moveDirect = (int)EDirect.WEST;
            }
            else if (targetCol == pivotCol + 1)
            {
                moveDirect = (int)EDirect.EAST;
            }
        }

        // 유령 리스폰
        private void GhostRespawn()
        {
            if (character.position == target.position)
            {
                if (coord.row == StageManager.Instance.ghostRespawnRow - 1 && coord.col == StageManager.Instance.ghostRespawnCol)
                {
                    isRespawn = false;
                    return;
                }

                InitBfsArray(ghostRespawnMovableCheckArray);
                PathFinding(coord, (StageManager.Instance.ghostRespawnRow - 1, StageManager.Instance.ghostRespawnCol));
                target = tileArray[coord.row, coord.col].transform;
            }

            character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
        }

        // 유령 리턴
        private void GhostReturn()
        {
            if (character.position == target.position)
            {
                if (coord.row == StageManager.Instance.ghostRespawnRow + 1 && coord.col == StageManager.Instance.ghostRespawnCol)
                {
                    isReturn = false;
                    animator.SetBool("RETURN", false);
                    boxCollider.enabled = true;
                    
                    return;
                }

                InitBfsArray(ghostRespawnMovableCheckArray);
                PathFinding(coord, (StageManager.Instance.ghostRespawnRow + 1, StageManager.Instance.ghostRespawnCol));
                target = tileArray[coord.row, coord.col].transform;
            }

            character.position =
                Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime * 1.5f);
        }

        /// <summary>
        /// 가는 뱡향 일정 거리 이내에 팩맨이 있는지 확인
        /// </summary>
        private bool GhostLookPac()
        {
            var coord = Coord;

            for (int i = 0; i < 5; i++)
            {
                coord = moveDirect switch
                {
                    (int)EDirect.EAST  => (coord.row, coord.col + 1),
                    (int)EDirect.WEST  => (coord.row, coord.col - 1),
                    (int)EDirect.SOUTH => (coord.row + 1, coord.col),
                    (int)EDirect.NORTH => (coord.row - 1, coord.col),
                    _ => (coord.row, coord.col)
                };

                if (StageManager.SafeArray(movableCheckArray, coord.row, coord.col) && movableCheckArray[coord.row, coord.col])
                {
                    if (coord.row == pac.Coord.row && coord.col == pac.Coord.col)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }

            return false;
        }

        /// <summary>
        /// 팩맨 추적
        /// </summary>
        private void PathTracking()
        {
            if (SafeTarget(pac.transform))
            {
                if (character.position == target.position)
                {
                    InitBfsArray();
                    PathFinding(coord, pac.Coord);
                    target = tileArray[coord.row, coord.col].transform;
                }

                character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
            }
        }

        private IEnumerator GhostRespawnCoolTime()
        {
            yield return new WaitForSeconds(RespawnCoolTime);
            
            isRespawn = true;
        }

        private IEnumerator TrackingPacMan()
        {
            yield return new WaitForSeconds(TrackingTime);
            
            isLookPac = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Pac"))
            {
                if (pac.GetIsSuperMode)
                {
                    boxCollider.SafeSetEnable(false);
                    
                    isReturn = true;
                    
                    animator.SetBool("RETURN", true);
                }
            }
        }
    }
}
