using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Transform[,] tileArray;
    protected bool[,] movableCheckArray;

    protected Transform target;
    protected Transform character;

    protected const int column = 23;
    protected const int line = 29;

    protected int row;
    protected int col;

    protected int moveDirect;

    protected float speed = 2f;

    public void InitCharacter()
    {

        character = this.transform;
        tileArray = MapManager.Instance.tileArray;
        movableCheckArray = MapManager.Instance.movableCheckArray;

        row = 2;
        col = 2;

        target = tileArray[row,col];
        character.position = target.position;
    }

    // 캐릭터 초기화 오버로딩
    public void InitCharacter(int x, int y)
    {
        character = this.transform;
        tileArray = MapManager.Instance.tileArray;
        movableCheckArray = MapManager.Instance.movableCheckArray;

        row = x;
        col = y;

        target = tileArray[row, col];
        character.position = target.position;
        
    }


    // 각 캐릭터 별 재정의 함수
    protected virtual void CharacterMove()
    {

    }

}
