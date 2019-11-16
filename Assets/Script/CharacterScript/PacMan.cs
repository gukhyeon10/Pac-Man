using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : CharacterBase
{
    int inputDirect = (int)EDirect.EAST;
    bool isInput = false;

    void Start()
    {
        pac = this;    
    }

    void Update()
    {
        KeyboardInput();
        CharacterMove();
    }

    void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isInput = true;
            inputDirect = (int)EDirect.WEST;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputDirect = (int)EDirect.EAST;
            isInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputDirect = (int)EDirect.NORTH;
            isInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputDirect = (int)EDirect.SOUTH;
            isInput = true;
        }
    }

    // 팩맨 이동 로직
    protected override void CharacterMove()
    {
        character.position = Vector3.MoveTowards(character.position, tileArray[row, col].transform.position, speed * Time.deltaTime);
        if (character.position == tileArray[row, col].transform.position)
        {
            if (!Warp()) // 반대편으로 전환하는게 아니라면
            {
                // 입력값에 의해 해당 방향 이동가능하면 방향전환
                switch (inputDirect)
                {
                    case (int)EDirect.EAST:
                        {
                            if (movableCheckArray[row, col + 1])
                            {
                                moveDirect = inputDirect;
                            }
                            break;
                        }
                    case (int)EDirect.WEST:
                        {
                            if (movableCheckArray[row, col - 1])
                            {
                                moveDirect = inputDirect;
                            }
                            break;
                        }
                    case (int)EDirect.SOUTH:
                        {
                            if (movableCheckArray[row + 1, col])
                            {
                                moveDirect = inputDirect;
                            }
                            break;
                        }
                    case (int)EDirect.NORTH:
                        {
                            if (movableCheckArray[row - 1, col])
                            {
                                moveDirect = inputDirect;
                            }
                            break;
                        }
                }

                //방향에 따라 목표 좌표 설정
                switch (moveDirect)
                {
                    case (int)EDirect.EAST:
                        {
                            if (movableCheckArray[row, col + 1])
                            {
                                col++;
                            }
                            break;
                        }
                    case (int)EDirect.WEST:
                        {
                            if (movableCheckArray[row, col - 1])
                            {
                                col--;
                            }
                            break;
                        }
                    case (int)EDirect.SOUTH:
                        {
                            if (movableCheckArray[row + 1, col])
                            {
                                row++;
                            }
                            break;
                        }
                    case (int)EDirect.NORTH:
                        {
                            if (movableCheckArray[row - 1, col])
                            {
                                row--;
                            }
                            break;
                        }
                }

            }
        }
        else
        {
            ReverseMove();  // 타일에서 타일 넘어가는 중에 반대 방향으로 전환 가능
        }
        isInput = false;
    }

    // 테두리 밖 이동시 반대편 워프
    bool Warp()
    {
        if (row == 0)
        {
            character.position = tileArray[line - 1, col].transform.position;
            row = line - 2;
            return true;
        }
        else if (row == line - 1)
        {
            character.position = tileArray[0, col].transform.position;
            row = 1;
            return true;
        }
        else if (col == 0)
        {
            character.position = tileArray[row, column - 1].transform.position;
            col = column - 2;
            return true;
        }
        else if (col == column - 1)
        {
            character.position = tileArray[row, 0].transform.position;
            col = 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    // 역방향 이동
    void ReverseMove()
    {
        if (isInput)
        {
            if (inputDirect == (int)EDirect.EAST && moveDirect == (int)EDirect.WEST)
            {
                col++;
                moveDirect = inputDirect;
            }
            else if (inputDirect == (int)EDirect.WEST && moveDirect == (int)EDirect.EAST)
            {
                col--;
                moveDirect = inputDirect;
            }
            else if (inputDirect == (int)EDirect.SOUTH && moveDirect == (int)EDirect.NORTH)
            {
                row++;
                moveDirect = inputDirect;
            }
            else if (inputDirect == (int)EDirect.NORTH && moveDirect == (int)EDirect.SOUTH)
            {
                row--;
                moveDirect = inputDirect;
            }
        }
    }

    //이동 중에 유령이랑 부딪히면 게임오버
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ghost"))
        {
            StageManager.Instance.StageResult((int)EResult.GAME_OVER);
        }
    }

    // 방향키 입력
    public void PacLeft()
    {
        inputDirect = (int)EDirect.WEST;
        isInput = true;
    }

    public void PacRight()
    {
        inputDirect = (int)EDirect.EAST;
        isInput = true;
    }

    public void PacUp()
    {
        inputDirect = (int)EDirect.NORTH;
        isInput = true;
    }

    public void PacDown()
    {
        inputDirect = (int)EDirect.SOUTH;
        isInput = true;
    }

}