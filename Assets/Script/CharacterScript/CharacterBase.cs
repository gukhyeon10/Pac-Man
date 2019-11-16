﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected GameTile[,] tileArray;
    protected bool[,] movableCheckArray;
    protected bool[,] ghostRespawnMovableCheckArray;
    protected List<int> movableList = new List<int>();   // 갈 수 있는 방향 리스트

    //이동 목표 Transform
    protected Transform target;
    protected Transform character;
    protected static CharacterBase pac;

    //타일 29행 23열
    protected const int column = 23;
    protected const int line = 29;

    protected AStarNode[,] AStarCheckArray = new AStarNode[line, column];
    protected Queue<AStarNode> openQueue = new Queue<AStarNode>();

    //목표 위치 좌표
    public int row;
    public int col;
    protected bool isRespawn = false, isReturn = false;

    protected float speed = 2f;

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
    }

    void InitBfsCheckArray()
    {
        for(int i=0; i<line; i++)
        {
            for(int j = 0; j< column; j++)
            {
                AStarCheckArray[i, j] = new AStarNode(i, j, i, j, 99999);
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

        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);

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
                                break;
                            }
                        case (int)EDirect.EAST:
                            {
                                col++;
                                break;
                            }
                        case (int)EDirect.NORTH:
                            {
                                row--;
                                break;
                            }
                        case (int)EDirect.SOUTH:
                            {
                                row++;
                                break;
                            }
                    }

                    if(StageManager.SafeArray<GameTile>(tileArray, row, col))
                    {
                        target = tileArray[row, col].transform;
                    }
                    else
                    {
                        target = null;
                    }

                }

            }
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

            InitBfsCheckArray();
            PathFinding(this.row, this.col, StageManager.Instance.ghostRespawnRow - 1, StageManager.Instance.ghostRespawnCol, ghostRespawnMovableCheckArray);
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
                return;
            }

            InitBfsCheckArray();
            PathFinding(this.row, this.col, StageManager.Instance.ghostRespawnRow + 1, StageManager.Instance.ghostRespawnCol, ghostRespawnMovableCheckArray);
            target = tileArray[row, col].transform;
        }
        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
    }

    // 팩맨 추적 경로
    protected void PathTracking()
    {
        if (SafeTarget(pac.transform))
        {
            if (character.position == target.position)
            {
                InitBfsCheckArray();
                PathFinding(this.row, this.col, pac.row, pac.col, movableCheckArray);
                target = tileArray[row, col].transform;
            }

            character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
        }
    }

    // 최단 경로 찾기
    void PathFinding(int row, int col, int targetRow, int targetCol, bool[,] movableCheckArray)
    {
        AStarNode startNode = new AStarNode(row, col, row, col, 0);
        openQueue.Clear();
        openQueue.Enqueue(startNode);

        while(openQueue.Count>0)
        {
            AStarNode node = openQueue.Dequeue();

            if(node.row == targetRow && node.col == targetCol)
            {
                int count = node.count;
                AStarNode reverse = node;
                for(int i=0; i< count; i++)
                {
                    int preRow = AStarCheckArray[reverse.row, reverse.col].preRow;
                    int preCol = AStarCheckArray[reverse.row, reverse.col].preCol;

                    if (preRow == this.row && preCol == this.col)
                    {
                        this.row = reverse.row;
                        this.col = reverse.col;
                        
                        return;
                    }
                    else
                    {
                        reverse.row = preRow;
                        reverse.col = preCol;
                    }
                    
                }
                break;
            }

            if(node.row -1 > 0 && movableCheckArray[node.row - 1, node.col] && AStarCheckArray[node.row - 1, node.col].count > node.count + 1)
            {
                AStarCheckArray[node.row - 1, node.col] = new AStarNode(node.row - 1, node.col, node.row, node.col, node.count + 1);
                openQueue.Enqueue(AStarCheckArray[node.row - 1, node.col]);
            }

            if (node.row + 1< line && movableCheckArray[node.row + 1, node.col] && AStarCheckArray[node.row + 1, node.col].count > node.count + 1)
            {
                AStarCheckArray[node.row + 1, node.col] = new AStarNode(node.row + 1, node.col, node.row, node.col, node.count + 1);
                openQueue.Enqueue(AStarCheckArray[node.row + 1, node.col]);
            }

            if (node.col - 1 > 0 && movableCheckArray[node.row, node.col - 1] && AStarCheckArray[node.row, node.col - 1].count > node.count + 1)
            {
                AStarCheckArray[node.row, node.col - 1] = new AStarNode(node.row, node.col - 1, node.row, node.col, node.count + 1);
                openQueue.Enqueue(AStarCheckArray[node.row, node.col-1]);
            }

            if (node.col + 1< column && movableCheckArray[node.row, node.col + 1] && AStarCheckArray[node.row, node.col + 1].count > node.count + 1)
            {
                AStarCheckArray[node.row, node.col + 1] = new AStarNode(node.row, node.col + 1, node.row, node.col, node.count + 1);
                openQueue.Enqueue(AStarCheckArray[node.row, node.col + 1]);
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
            Debug.Log("Pac is null");
            return false;
        }
    }
}