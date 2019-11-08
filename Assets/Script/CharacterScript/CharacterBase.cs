using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected List<Transform> tileList;
    protected Dictionary<int, bool> dicMovable;

    protected Transform target;
    protected Transform character;

    protected const int columnCount = 21;

    protected int locationX;
    protected int locationY;

    protected int moveDirect;

    protected float speed = 2f;

    public void InitCharacter()
    {

        character = this.transform;
        tileList = MapManager.Instance.tileList;
        dicMovable = MapManager.Instance.dicMovable;

        locationX = 1;
        locationY = 1;

        target = tileList[locationX * columnCount + locationY];
        character.position = target.position;
    }

    // 캐릭터 초기화 오버로딩
    public void InitCharacter(int x, int y)
    {
        character = this.transform;
        tileList = MapManager.Instance.tileList;
        dicMovable = MapManager.Instance.dicMovable;

        locationX = x;
        locationY = y;

        target = tileList[locationX * columnCount + locationY];
        character.position = target.position;
        
    }


    // 각 캐릭터 별 재정의 함수
    protected virtual void CharacterMove()
    {

    }

}
