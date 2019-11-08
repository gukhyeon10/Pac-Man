using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : CharacterBase
{
    int inputDirect = 1;
    bool isTurn = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCorutine());
    }

    // 초기화 코루틴     --> Start에서 초기화안하고 구지 코루틴 사용해서 한 프레임 넘기고 나서 초기화 하는 이유는 다음과 같다.
                           // tile이 grid layout에 의해서 아직 정렬되지 않을때 특정 타일의 포지션을 참조하여 초기화하는 듯하다(초기화 위치가 계속해서 버그가 남)
                           // 그러기때문에 한 프레임 텀을 주고 위치 초기화
    IEnumerator InitCorutine()
    {
        yield return null;
        InitCharacter(2, 2);
    }

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
