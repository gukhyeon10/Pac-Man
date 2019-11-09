using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : CharacterBase
{
    int inputDirect = 1;
    bool isTurn = false;


    void Update()
    {
        CharacterMove();    
    }

    // 팩맨 이동 로직
    protected override void CharacterMove()
    {
        if (target == null) // 타겟 null 처리
        {
            return;
        }

        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);

        if(character.position == target.position)
        {
            if(row==0)
            {
                character.position = tileArray[line - 1, col].position;
                row = line - 2;
                target = tileArray[row, col];
            }
            else if(row == line-1)
            {
                character.position = tileArray[0, col].position;
                row = 1;
                target = tileArray[row, col];
            }
            else if(col == 0)
            {
                character.position = tileArray[row, column-1].position;
                col = column-2;
                target = tileArray[row, col];
            }
            else if(col == column -1)
            {
                character.position = tileArray[row, 0].position;
                col = 1;
                target = tileArray[row, col];
            }
            else
            {
                isTurn = true;
                switch (inputDirect)
                {
                    case 0:
                        {
                            if (movableCheckArray[row, col - 1])
                            {
                                col--;
                            }
                            else
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (movableCheckArray[row, col + 1])
                            {
                                col++;
                            }
                            else
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (movableCheckArray[row - 1, col])
                            {
                                row--;
                            }
                            else
                            {
                                isTurn = false;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (movableCheckArray[row + 1, col])
                            {
                                row++;
                            }
                            else
                            {
                                isTurn = false;
                            }
                            break;
                        }
                }

                if (isTurn)
                {
                    target = tileArray[row, col];
                    moveDirect = inputDirect;
                }
                else
                {
                    switch (moveDirect)
                    {
                        case 0:
                            {
                                if (movableCheckArray[row, col - 1])
                                {
                                    col--;
                                }
                                break;
                            }
                        case 1:
                            {
                                if (movableCheckArray[row, col + 1])
                                {
                                    col++;
                                }
                                break;
                            }
                        case 2:
                            {
                                if (movableCheckArray[row - 1, col])
                                {
                                    row--;
                                }
                                break;
                            }
                        case 3:
                            {
                                if (movableCheckArray[row + 1, col])
                                {
                                    row++;
                                }
                                break;
                            }
                    }
                    target = tileArray[row, col];
                }
            }
            
        }
    }

    // 방향키 입력
    public void PacLeft()
    {
        inputDirect = 0;
    }

    public void PacRight()
    {
        inputDirect = 1;
    }

    public void PacUp()
    {
        inputDirect = 2;
    }

    public void PacDown()
    {
        inputDirect = 3;
    }

}
