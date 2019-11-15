using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected GameTile[,] tileArray;
    protected bool[,] movableCheckArray;
    protected List<int> movableList = new List<int>();   // 갈 수 있는 방향 리스트

    //이동 목표 Transform
    protected Transform target;
    protected Transform character;

    //타일 29행 23열
    protected const int column = 23;
    protected const int line = 29;

    //목표 위치 좌표
    protected int row;
    protected int col;

    protected float speed = 2f;

    public void InitCharacter()
    {

        character = this.transform;
        tileArray = StageManager.Instance.tileArray;
        movableCheckArray = StageManager.Instance.movableCheckArray;

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

        row = x;
        col = y;

        target = tileArray[row, col].transform;
        character.position = target.position;
    }


    // 각 캐릭터 별 재정의 함수
    protected virtual void CharacterMove()
    {
        // 기본 움직임 (랜덤 방향)
        if (target == null) // 타겟 null 처리
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

}