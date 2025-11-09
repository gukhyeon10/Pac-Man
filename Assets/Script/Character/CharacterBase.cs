using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected GameTile[,] tileArray;
    protected bool[,] movableCheckArray; // 캐릭터 이동 가능 좌표 플래그
    protected bool[,] ghostRespawnMovableCheckArray; // 유령 리스폰 경로 이동 가능 좌표 플래그
    protected List<int> movableList = new List<int>();   // 갈 수 있는 방향 리스트
    
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
    public int row;
    public int col;
    protected bool isRespawn = false; 
    
    protected bool isReturn = false;
    protected float respawnCoolTime = 10f;
    
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

        row = 2;
        col = 2;

        target = tileArray[row, col].transform;
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

        row = x;
        col = y;

        target = tileArray[row, col].transform;
        character.position = target.position;

        isRespawn = false;
        isReturn = false;
        boxCollider.enabled = true;
    }

    void InitBfsCheckArray(bool[,] movableCheckArray = null)
    {
        this.movableCheckArray = movableCheckArray ?? this.movableCheckArray;
        
        for (int i = 0; i < line; i++)
        {
            for(int j = 0; j< column; j++)
            {
                bfsArray[i, j].Modify(i, j, i, j , false);
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

        if (character.position == target.position)
        {
            if (row == 0)
            {
                character.position = tileArray[line - 1, col].transform.position;
                row = line - 2;
                target = tileArray[row, col].transform;
            }
            else if (row == line - 1)
            {
                character.position = tileArray[0, col].transform.position;
                row = 1;
                target = tileArray[row, col].transform;
            }
            else if (col == 0)
            {
                character.position = tileArray[row, column - 1].transform.position;
                col = column - 2;
                target = tileArray[row, col].transform;
            }
            else if (col == column - 1)
            {
                character.position = tileArray[row, 0].transform.position;
                col = 1;
                target = tileArray[row, col].transform;
            }
            else
            {
                bool isTurn = true;
                switch(moveDirect)
                {
                    case (int)EDirect.EAST:
                        {
                            if (movableCheckArray[row, col + 1])
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case (int)EDirect.WEST:
                        {
                            if (movableCheckArray[row, col - 1])
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case (int)EDirect.SOUTH:
                        {
                            if (movableCheckArray[row + 1, col])
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case (int)EDirect.NORTH:
                        {
                            if (movableCheckArray[row - 1, col])
                            {
                                isTurn = false;
                            }
                            break;
                        }
                }

                if(isTurn)
                {
                    movableList.Clear();
                    if (movableCheckArray[row, col - 1])
                    {
                        movableList.Add((int)EDirect.WEST);
                    }
                    if (movableCheckArray[row, col + 1])
                    {
                        movableList.Add((int)EDirect.EAST);
                    }
                    if (movableCheckArray[row - 1, col])
                    {
                        movableList.Add((int)EDirect.NORTH);
                    }
                    if (movableCheckArray[row + 1, col])
                    {
                        movableList.Add((int)EDirect.SOUTH);
                    }

                    if (movableList.Count > 0)
                    {
                        int index = Random.Range(0, movableList.Count);
                        switch (movableList[index])
                        {
                            case (int)EDirect.WEST:
                                {
                                    col--;
                                    moveDirect = (int)EDirect.WEST;
                                    break;
                                }
                            case (int)EDirect.EAST:
                                {
                                    col++;
                                    moveDirect = (int)EDirect.EAST;
                                    break;
                                }
                            case (int)EDirect.NORTH:
                                {
                                    row--;
                                    moveDirect = (int)EDirect.NORTH;
                                    break;
                                }
                            case (int)EDirect.SOUTH:
                                {
                                    row++;
                                    moveDirect = (int)EDirect.SOUTH;
                                    break;
                                }
                        }

                    }
                }
                else
                {
                    switch (moveDirect)
                    {
                        case (int)EDirect.EAST:
                            {
                                col++;
                                moveDirect = (int)EDirect.EAST;
                                break;
                            }
                        case (int)EDirect.WEST:
                            {
                                col--;
                                moveDirect = (int)EDirect.WEST;
                                break;
                            }
                        case (int)EDirect.SOUTH:
                            {
                                row++;
                                moveDirect = (int)EDirect.SOUTH;
                                break;
                            }
                        case (int)EDirect.NORTH:
                            {
                                row--;
                                moveDirect = (int)EDirect.NORTH;
                                break;
                            }
                    }
                }

                if (StageManager.SafeArray<GameTile>(tileArray, row, col))
                {
                    target = tileArray[row, col].transform;
                }
                else
                {
                    target = null;
                }

            }
        }

        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
    }

    //방향 전환 시 어느 방향인지 인지 함수
    protected void GhostTurnCheck(int targetRow, int targetCol, int pivotRow, int pivotCol)
    {
        if(targetRow == pivotRow - 1)
        {
            moveDirect = (int)EDirect.NORTH;
        }
        else if(targetRow == pivotRow + 1)
        {
            moveDirect = (int)EDirect.SOUTH;
        }
        else if(targetCol == pivotCol - 1)
        {
            moveDirect = (int)EDirect.WEST;
        }
        else if(targetCol == pivotCol + 1)
        {
            moveDirect = (int)EDirect.EAST;
        }
    }

    // 유령 리스폰
    protected void GhostRespawn()
    {
        if(character.position == target.position)
        {
            if (row == StageManager.Instance.ghostRespawnRow-1 && col == StageManager.Instance.ghostRespawnCol)
            { 
                isRespawn = false;
                return;
            }

            InitBfsCheckArray(ghostRespawnMovableCheckArray);
            PathFinding(this.row, this.col, StageManager.Instance.ghostRespawnRow - 1, StageManager.Instance.ghostRespawnCol);
            target = tileArray[row, col].transform;
        }

        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
    }

    // 유령 리턴
    protected void GhostReturn()
    {
        if(character.position == target.position)
        {
            if(row == StageManager.Instance.ghostRespawnRow + 1 && col == StageManager.Instance.ghostRespawnCol)
            {
                isReturn = false;
                animator.SetBool("RETURN", false);
                boxCollider.enabled = true;

                respawnCoolTime = 10f;
                return;
            }

            InitBfsCheckArray(ghostRespawnMovableCheckArray);
            PathFinding(this.row, this.col, StageManager.Instance.ghostRespawnRow + 1, StageManager.Instance.ghostRespawnCol);
            target = tileArray[row, col].transform;
        }
        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime * 1.5f);
    }

    // 가는 뱡향 일정 거리 이내에 팩맨이 있는지 확인
    protected bool GhostLookPac()
    {
        int lookRow, lookCol;
        lookRow = this.row;
        lookCol = this.col;

        for(int i=0; i<5; i++)
        {
            switch (moveDirect)
            {
                case (int)EDirect.EAST:
                    {
                        lookCol++;
                        break;
                    }
                case (int)EDirect.WEST:
                    {
                        lookCol--;
                        break;
                    }
                case (int)EDirect.SOUTH:
                    {
                        lookRow++;
                        break;
                    }
                case (int)EDirect.NORTH:
                    {
                        lookRow--;
                        break;
                    }
            }

            if(StageManager.SafeArray(movableCheckArray, lookRow, lookCol) && movableCheckArray[lookRow, lookCol])
            {
                if (lookRow == pac.row && lookCol == pac.col)
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

    // 팩맨 추적 경로
    protected void PathTracking()
    {
        if (SafeTarget(pac.transform))
        {
            if (character.position == target.position)
            {
                InitBfsCheckArray();
                PathFinding(this.row, this.col, pac.row, pac.col);
                target = tileArray[row, col].transform;
            }

            character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
        }
    }
    

    /// <summary>
    /// 타겟 좌표 경로 탐색 시작
    /// </summary>
    private void PathFinding(int row, int col, int targetRow, int targetCol)
    {
        bfsArray[row, col].Modify(row, col, row, col, true);
        
        bfsQueue.Clear();
        bfsQueue.Enqueue(bfsArray[row, col]);

        while (bfsQueue.Count > 0)
        {
            var node = bfsQueue.Dequeue();
            if (node.row == targetRow && node.col == targetCol)
            {
                BackTracking(node.row, node.col);
                break;
            }

            PathFindingProcess(node.row - 1,  node.col, node.row, node.col);
            
            PathFindingProcess(node.row + 1,  node.col, node.row, node.col);
            
            PathFindingProcess(node.row,  node.col - 1, node.row, node.col);
            
            PathFindingProcess(node.row,  node.col + 1, node.row, node.col);
        }
    }

    /// <summary>
    /// 경로 탐색 처리
    /// </summary>
    private void PathFindingProcess(int destRow, int destCol, int srcRow, int srcCol)
    {
        if (bfsArray[destRow, destCol].isVisited == false && IsMovable(destRow, destCol))
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
        if (row > 0 && col > 0 && row < line && col < column)
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
        
        while(!(reverseRow == this.row && reverseCol == this.col))
        {
            int preRow = bfsArray[reverseRow, reverseCol].preRow;
            int preCol = bfsArray[reverseRow, reverseCol].preCol;

            if (preRow == this.row && preCol == this.col)
            {
                GhostTurnCheck(reverseRow, reverseCol, this.row, this.col);

                this.row = reverseRow;
                this.col = reverseCol;

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
    


    // 타겟 방어 코드
    protected bool SafeTarget<T>(T target)
    {
        if(target != null)
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