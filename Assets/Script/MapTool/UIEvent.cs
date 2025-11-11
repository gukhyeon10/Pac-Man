using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGame;

public class UIEvent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public bool isWall;
    public bool isItem;
    public bool isCharacter;

    public EWall objectNumber;

    // 맵툴 화면 아래 타일 클릭
    public void OnMouseDown()
    {
        if(spriteRenderer != null)
        {
            if(isWall)
            {
                MapToolCursor.Instance.cursorSprite = spriteRenderer.sprite;
                MapToolCursor.Instance.cursorType = EObjectType.WALL;
                Debug.Log("Cursor Change : " + MapToolCursor.Instance.cursorSprite.name);
            }
            else if(isItem)
            {
                MapToolCursor.Instance.cursorSprite = spriteRenderer.sprite;
                MapToolCursor.Instance.cursorType = EObjectType.ITEM;
                Debug.Log("Cursor Change : " + MapToolCursor.Instance.cursorSprite.name);
            }
            else if(isCharacter)
            {
                MapToolCursor.Instance.cursorSprite = spriteRenderer.sprite;
                MapToolCursor.Instance.cursorType = EObjectType.CHARACTER;
                Debug.Log("Cursor Change : " + MapToolCursor.Instance.cursorSprite.name);
            }

            MapToolCursor.Instance.objectNumber = objectNumber;
        
        }
        else
        {
            Debug.Log("Select Sprite is null");
        }
    }

}
