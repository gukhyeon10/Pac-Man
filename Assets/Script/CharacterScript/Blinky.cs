using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : CharacterBase
{

    Random r = new Random();
    bool isTurn = false;
    List<int> movableList = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCorutine());
    }

    IEnumerator InitCorutine()
    {
        yield return null;
        InitCharacter(10, 10);
    }

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

            movableList.Clear();
            if (dicMovable[locationX * columnCount + locationY - 1])
            {
                movableList.Add(0);
            }
            if (dicMovable[locationX * columnCount + locationY + 1])
            {
                movableList.Add(1);
            }
            if (dicMovable[(locationX - 1) * columnCount + locationY])
            {
                movableList.Add(2);
            }
            if (dicMovable[(locationX + 1) * columnCount + locationY])
            {
                movableList.Add(3);
            }

            if(movableList.Count > 0)
            {
                int index = Random.Range(0, movableList.Count);
                switch(movableList[index])
                {
                    case 0:
                        {
                            target = tileList[locationX * columnCount + locationY - 1];
                            locationY--;
                            break;
                        }
                    case 1:
                        {
                            target = tileList[locationX * columnCount + locationY + 1];
                            locationY++;
                            break;
                        }
                    case 2:
                        {
                            target = tileList[(locationX - 1) * columnCount + locationY];
                            locationX--;
                            break;
                        }
                    case 3:
                        {
                            target = tileList[(locationX + 1) * columnCount + locationY];
                            locationX++;
                            break;
                        }
                }
            }

        }
    }
}
