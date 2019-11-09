using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵툴 그리드내 타일 이벤트
public class TileEvent : MonoBehaviour
{
    MapToolCursor cursor;   // 커서 인스턴스

    SpriteRenderer spriteRenderer; //SpriteRenderer 컴포넌트
    Sprite currentTileSprite;      //현재 타일 스프라이트

    public int objectType = (int)EObjectType.WALL;

    float rot;

    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.transform.eulerAngles = Vector3.zero;
        InitTile((int)EObjectType.WALL);
        cursor = MapToolCursor.Instance;
    }


    public void InitTile(int objectType)
    {
        //타일 정보 초기화
        currentTileSprite = spriteRenderer.sprite;
        rot = this.transform.eulerAngles.z;
        this.objectType = objectType;
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

            if (cursor.cursorType != (int)EObjectType.WALL)
            {
                rot = 0f;
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
    {
        currentTileSprite = cursor.cursorSprite;
        rot = cursor.rot;
        objectType = cursor.cursorType;

        if (objectType != (int)EObjectType.WALL)
        {
            rot = 0f;
        }


    }


}
