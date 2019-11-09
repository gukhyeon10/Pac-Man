using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected Transform[,] tileArray;
    protected bool[,] movableCheckArray;

    //이동 목표 Transform
    protected Transform target;
    protected Transform character;

    //타일 29행 23열
    protected const int column = 23;
    protected const int line = 29;

    //목표 위치 좌표
    protected int row;
    protected int col;

    protected int moveDirect;

    protected float speed = 2f;

    public void InitCharacter()
    {

        character = this.transform;
        tileArray = StageManager.Instance.tileArray;
        movableCheckArray = StageManager.Instance.movableCheckArray;

        row = 2;
        col = 2;

        target = tileArray[row,col];
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

        target = tileArray[row, col];
        character.position = target.position;
    }


    // 각 캐릭터 별 재정의 함수
    protected virtual void CharacterMove()
    {
    }

}
