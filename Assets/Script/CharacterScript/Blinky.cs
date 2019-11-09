using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : CharacterBase
{
    
    bool isTurn = false;
    List<int> movableList = new List<int>();
    

    // Update is called once per frame
    void Update()
    {
        CharacterMove();
    }


    // 블링키 AI
    protected override void CharacterMove()
    {
        if (target == null) // 타겟 null 처리
        {
            return;
        }

        character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);

        if (character.position == target.position)
        {
            if (row == 0)
            {
                character.position = tileArray[line - 1, col].position;
                row = line - 2;
                target = tileArray[row, col];
            }
            else if (row == line - 1)
            {
                character.position = tileArray[0, col].position;
                row = 1;
                target = tileArray[row, col];
            }
            else if (col == 0)
            {
                character.position = tileArray[row, column - 1].position;
                col = column - 2;
                target = tileArray[row, col];
            }
            else if (col == column - 1)
            {
                character.position = tileArray[row, 0].position;
                col = 1;
                target = tileArray[row, col];
            }
            else
            {
                movableList.Clear();
                if (movableCheckArray[row, col - 1])
                {
                    movableList.Add(0);
                }
                if (movableCheckArray[row, col + 1])
                {
                    movableList.Add(1);
                }
                if (movableCheckArray[row - 1, col])
                {
                    movableList.Add(2);
                }
                if (movableCheckArray[row + 1, col])
                {
                    movableList.Add(3);
                }

                if (movableList.Count > 0)
                {
                    int index = Random.Range(0, movableList.Count);
                    switch (movableList[index])
                    {
                        case 0:
                            {
                                col--;
                                break;
                            }
                        case 1:
                            {
                                col++;
                                break;
                            }
                        case 2:
                            {
                                row--;
                                break;
                            }
                        case 3:
                            {
                                row++;
                                break;
                            }
                    }

                    target = tileArray[row, col];
                }

            }


        }
    }
}
