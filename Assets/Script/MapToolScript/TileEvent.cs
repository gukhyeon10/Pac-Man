using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵툴 그리드내 타일 이벤트
public class TileEvent : MonoBehaviour
{
    MapToolCursor cursor;   // 커서 인스턴스

    SpriteRenderer spriteRenderer; //SpriteRenderer 컴포넌트
    public Sprite currentTileSprite;      //현재 타일 스프라이트
    Sprite defaultSprite;          //디폴트 스프라이트

    public int objectType = (int)EObjectType.WALL;
    public int objectNumber = (int)EWall.DEFAULT;
    public float rot;

    public int row, col;

    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;

        this.transform.eulerAngles = Vector3.zero;
        InitTile((int)EObjectType.WALL, (int)EWall.DEFAULT, defaultSprite);
        cursor = MapToolCursor.Instance;
    }

    public void InitTile()
    {
        //기본타일로 초기화
        this.transform.eulerAngles = Vector3.zero ;
        this.rot = 0f;
        InitTile((int)EObjectType.WALL, (int)EWall.DEFAULT, defaultSprite);
    }

    public void InitTile(int objectType, int objectNumber, Sprite sprite)
    {
        // 특정타일로 초기화 
        spriteRenderer.sprite = sprite;
        currentTileSprite = sprite;
        rot = this.transform.eulerAngles.z;
        this.objectType = objectType;
        this.objectNumber = objectNumber;
    }

    public void AutoTile(int objectType, int objectNumber, Sprite sprite, float rot)
    {
        // 상황에 맞는 타일로 자동완성 
        spriteRenderer.sprite = sprite;
        currentTileSprite = sprite;
        this.rot = rot;
        this.transform.eulerAngles = new Vector3(0f, 0f, rot);
        this.objectType = objectType;
        this.objectNumber = objectNumber;

    }

    //커서에 의해 미리보기
    public void OnMouseOver()
    {
        spriteRenderer.sprite = cursor.cursorSprite;

        this.transform.eulerAngles = new Vector3(0, 0, cursor.rot);

        if (cursor.cursorType != (int)EObjectType.WALL)
        {
            this.transform.eulerAngles = Vector3.zero;
        }
        
        //마우스 왼쪽 버튼 누르고 있는 상태 (드래그) 타일 정보 변환
        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentTileSprite = cursor.cursorSprite;
            rot = cursor.rot;
            objectType = cursor.cursorType;
            objectNumber = cursor.objectNumber;

            if (cursor.cursorType != (int)EObjectType.WALL)
            {
                rot = 0f;
            }

            if (cursor.cursorType == (int)EObjectType.WALL && objectNumber == (int)EWall.LINE)
            {
                MapToolManager.Instance.TileAutoComplete(this.row, this.col, true);
            }
        }
    }

    //커서가 벗어날 경우 원래의 타일(currentTileSprite)로 변환
    public void OnMouseExit()
    {
        spriteRenderer.sprite = currentTileSprite;
        this.transform.eulerAngles = new Vector3(0, 0, rot);

    }

    //클릭시 타일 정보 변환
    public void OnMouseDown()
    {/*
        currentTileSprite = cursor.cursorSprite;
        rot = cursor.rot;
        objectType = cursor.cursorType;
        objectNumber = cursor.objectNumber;

        if (objectType != (int)EObjectType.WALL)
        {
            rot = 0f;
        }
        
        if(objectType == (int)EObjectType.WALL && objectNumber == (int)EWall.LINE)
        {
            MapToolManager.Instance.TileAutoComplete(this.row, this.col);
        }
        */
    }

}
