using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGame;

//맵툴 그리드내 타일 이벤트
public class TileEvent : MonoBehaviour
{
    private MapToolCursor cursor;

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    public Sprite currentTileSprite;
    
    public EObjectType objectType = EObjectType.WALL;
    public EWall objectNumber = EWall.DEFAULT;
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

    public void InitTile(EObjectType objectType, EWall objectNumber, Sprite sprite)
    {
        // 특정타일로 초기화 
        spriteRenderer.sprite = sprite;
        currentTileSprite = sprite;
        rot = this.transform.eulerAngles.z;
        this.objectType = objectType;
        this.objectNumber = objectNumber;
    }

    public void AutoTile(EObjectType objectType, EWall objectNumber, Sprite sprite, float rot)
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

            //타일 자동완성 기능
            if (cursor.isAutoCompleteActive && cursor.cursorType == (int)EObjectType.WALL)
            {
                if(objectNumber == (int)EWall.DEFAULT)
                {
                    MapToolManager.Instance.TileAutoComplete(this.row, this.col, true, true);
                }
                // 유령 리스폰 지역 입구를 뜻하는 3종류의 타일은 자동완성 기능 제외.  나머지 타일들은 자동완성 
                else if(objectNumber != EWall.CENTERDOOR && objectNumber != EWall.RIGHTDOOR && objectNumber != EWall.LEFTDOOR)
                {
                    MapToolManager.Instance.TileAutoComplete(this.row, this.col, true, false);
                }
            }
        }
    }

    //커서가 벗어날 경우 원래의 타일(currentTileSprite)로 변환
    public void OnMouseExit()
    {
        spriteRenderer.sprite = currentTileSprite;
        this.transform.eulerAngles = new Vector3(0, 0, rot);

    }

}
